using System;
using System.Collections.Generic;
using FluentValidation;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.Validators
{
    public class CategoryDtoValidator : AbstractValidator<CategoryDto>
    {
        private ILocalizationService _localizationService = EngineContext.Current.Resolve<ILocalizationService>();

        public CategoryDtoValidator(string httpMethod, Dictionary<string, object> passedPropertyValuePaires)
        {
            if (string.IsNullOrEmpty(httpMethod) || httpMethod.Equals("post", StringComparison.InvariantCultureIgnoreCase))
            {
                SetNameRule();
            }
            else if (httpMethod.Equals("put", StringComparison.InvariantCultureIgnoreCase))
            {
                int parsedId = 0;

                RuleFor(x => x.Id)
                        .NotNull()
                        .NotEmpty()
                        .Must(id => int.TryParse(id, out parsedId) && parsedId > 0)
                        .WithMessage(_localizationService.GetResource("Admin.Catalog.Categories.Fields.Id.Invalid"));

                if (passedPropertyValuePaires.ContainsKey("name"))
                {
                    SetNameRule();
                }
            }
        }

        private void SetNameRule()
        {
            RuleFor(x => x.Name)
                        .NotNull()
                        .NotEmpty()
                        .WithMessage(_localizationService.GetResource("Admin.Catalog.Categories.Fields.Name.Required"));
        }
    }
}