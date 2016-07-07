using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentValidation;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.DTOs;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.Validators
{
    public class AddressDtoValidator : AbstractValidator<AddressDto>
    {
        private ILocalizationService _localizationService = EngineContext.Current.Resolve<ILocalizationService>();

        public AddressDtoValidator(string httpMethod, Dictionary<string, object> passedPropertyValuePaires)
        {
            if (string.IsNullOrEmpty(httpMethod) ||
                httpMethod.Equals("post", StringComparison.InvariantCultureIgnoreCase))
            {
                SetNotNullOrEmptyRule(dto => dto.FirstName,
                    _localizationService.GetResource("admin.address.fields.firstname.required"));

                SetNotNullOrEmptyRule(dto => dto.LastName,
                   _localizationService.GetResource("admin.address.fields.lastname.required"));

                SetNotNullOrEmptyRule(dto => dto.Email,
                   _localizationService.GetResource("admin.address.fields.email.required"));

                SetNotNullOrEmptyRule(dto => dto.CountryId <= 0 ? string.Empty : dto.CountryId.ToString(),
                  _localizationService.GetResource("admin.address.fields.country.required"));

                SetNotNullOrEmptyRule(dto => dto.City,
                  _localizationService.GetResource("admin.address.fields.city.required"));

                SetNotNullOrEmptyRule(dto => dto.Address1,
                    _localizationService.GetResource("admin.address.fields.address1.required"));

                SetNotNullOrEmptyRule(dto => dto.ZipPostalCode,
                   _localizationService.GetResource("admin.address.fields.zippostalcode.required"));

                SetNotNullOrEmptyRule(dto => dto.PhoneNumber,
                   _localizationService.GetResource("admin.address.fields.phonenumber.required"));
            }
            else if (httpMethod.Equals("put", StringComparison.InvariantCultureIgnoreCase))
            {
                int parsedId = 0;

                RuleFor(x => x.Id)
                    .NotNull()
                    .NotEmpty()
                    .Must(id => int.TryParse(id, out parsedId) && parsedId > 0)
                    .WithMessage(_localizationService.GetResource("Api.Customers.Fields.Id.Invalid"));

                SetNotNullOrEmptyRuleIfPassed("first_name",
                    _localizationService.GetResource("admin.address.fields.firstname.required"), passedPropertyValuePaires);

                SetNotNullOrEmptyRuleIfPassed("last_name",
                   _localizationService.GetResource("admin.address.fields.lastname.required"), passedPropertyValuePaires);

                SetNotNullOrEmptyRuleIfPassed("email",
                   _localizationService.GetResource("admin.address.fields.email.required"), passedPropertyValuePaires);

                SetNotNullOrEmptyRuleIfPassed("city",
                   _localizationService.GetResource("admin.address.fields.city.required"), passedPropertyValuePaires);

                SetNotNullOrEmptyRuleIfPassed("address1",
                   _localizationService.GetResource("admin.address.fields.address1.required"), passedPropertyValuePaires);

                SetNotNullOrEmptyRuleIfPassed("zip_postal_code",
                    _localizationService.GetResource("admin.address.fields.zippostalcode.required"), passedPropertyValuePaires);

                SetNotNullOrEmptyRuleIfPassed("phone",
                   _localizationService.GetResource("admin.address.fields.phonenumber.required"), passedPropertyValuePaires);
            }
        }

        private void SetNotNullOrEmptyRuleIfPassed(string field, string message, Dictionary<string, object> passedPropertyValuePaires)
        {
            if (passedPropertyValuePaires.ContainsKey(field))
            {
                SetNotNullOrEmptyRule(dto => dto.FirstName, message);
            }
        }

        private void SetNotNullOrEmptyRule(Expression<Func<AddressDto, string>> expression, string errorMessage)
        {
            RuleFor(expression)
                   .NotNull()
                   .NotEmpty()
                   .WithMessage(errorMessage);
        }
    }
}