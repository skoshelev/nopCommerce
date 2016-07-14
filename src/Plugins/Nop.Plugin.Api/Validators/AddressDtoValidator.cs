using System;
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

        public AddressDtoValidator()
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
        
        private void SetNotNullOrEmptyRule(Expression<Func<AddressDto, string>> expression, string errorMessage)
        {
            RuleFor(expression)
                   .NotNull()
                   .NotEmpty()
                   .WithMessage(errorMessage);
        }
    }
}