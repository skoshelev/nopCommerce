using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.Helpers;
using Nop.Services.Customers;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.Validators
{
    public class CustomerDtoValidator : AbstractValidator<CustomerDto>
    {
        private ILocalizationService _localizationService = EngineContext.Current.Resolve<ILocalizationService>();
        private ICustomerRolesHelper _customerRolesHelper = EngineContext.Current.Resolve<ICustomerRolesHelper>();
        private ICustomerService _customerService = EngineContext.Current.Resolve<ICustomerService>();

        public CustomerDtoValidator(string httpMethod, Dictionary<string, object> passedPropertyValuePaires)
        {
            if (string.IsNullOrEmpty(httpMethod) ||
                httpMethod.Equals("post", StringComparison.InvariantCultureIgnoreCase))
            {
                SetRuleForRoles();
            }
            else if (httpMethod.Equals("put", StringComparison.InvariantCultureIgnoreCase))
            {
                int parsedId = 0;

                RuleFor(x => x.Id)
                    .NotNull()
                    .NotEmpty()
                    .Must(id => int.TryParse(id, out parsedId) && parsedId > 0)
                    .WithMessage(_localizationService.GetResource("Api.Customers.Fields.Id.Invalid"))
                    .DependentRules(dto => dto.RuleFor(customer => customer).Must(customer =>
                        {
                            return _customerService.GetCustomerById(parsedId) != null; 
                        })
                        .WithMessage("Customer not found"));

                // TODO: think of a way to not hardcode the json property name.
                if (passedPropertyValuePaires.ContainsKey("role_ids"))
                {
                    SetRuleForRoles();
                }
            }

            if (passedPropertyValuePaires.ContainsKey("password"))
            {
                RuleForEach(customer => customer.Password)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage(_localizationService.GetResource("Account.ChangePassword.Errors.PasswordIsNotProvided"));
            }

            // The fields below are not required, but if they are passed they should be validated.
            if (passedPropertyValuePaires.ContainsKey("billing_address"))
            {
                RuleFor(x => x.BillingAddress)
                    .SetValidator(new AddressDtoValidator());
            }

            if (passedPropertyValuePaires.ContainsKey("shipping_address"))
            {
                RuleFor(x => x.ShippingAddress)
                    .SetValidator(new AddressDtoValidator());
            }

            if (passedPropertyValuePaires.ContainsKey("addresses"))
            {
                RuleForEach(x => x.CustomerAddresses)
                    .SetValidator(new AddressDtoValidator());
            }
        }

        private void SetRuleForRoles()
        {
            IList<CustomerRole> customerRoles = null;

            RuleFor<List<int>>(x => x.RoleIds)
                   .NotNull()
                   .Must(roles => roles.Count > 0)
                   .WithMessage(_localizationService.GetResource("Api.Customers.Fields.RoleIds.Required"))
                   .DependentRules(dependentRules => dependentRules.RuleFor(dto => dto.RoleIds)
                       .Must(roleIds =>
                       {
                           if (customerRoles == null)
                           {
                               customerRoles = _customerRolesHelper.GetValidCustomerRoles(roleIds);
                           }

                           bool isInGuestAndRegisterRoles = _customerRolesHelper.IsInGuestsRole(customerRoles) &&
                                                            _customerRolesHelper.IsInRegisteredRole(customerRoles);

                           // Customer can not be in guest and register roles simultaneously
                           return !isInGuestAndRegisterRoles;
                       })
                       .WithMessage(_localizationService.GetResource("Api.Customers.Fields.RoleIds.MustNotBeInGuestAndRegisterRolesSimultaneously"))
                       .DependentRules(dependentRule => dependentRules.RuleFor(dto => dto.RoleIds)
                            .Must(roleIds =>
                            {
                                if (customerRoles == null)
                                {
                                    customerRoles = _customerRolesHelper.GetValidCustomerRoles(roleIds);
                                }

                                bool isInGuestOrRegisterRoles = _customerRolesHelper.IsInGuestsRole(customerRoles) ||
                                                                _customerRolesHelper.IsInRegisteredRole(customerRoles);

                                // Customer must be in either guest or register role.
                                return isInGuestOrRegisterRoles;
                            })
                            .WithMessage(_localizationService.GetResource("Api.Customers.Fields.RoleIds.MustBeInGuestOrRegisterRole"))
                       )
                   );
        }
    }
}