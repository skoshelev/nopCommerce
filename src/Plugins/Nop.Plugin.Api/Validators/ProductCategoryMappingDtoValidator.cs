using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.DTOs.ProductCategoryMappings;
using Nop.Services.Catalog;
using Nop.Services.Localization;

namespace Nop.Plugin.Api.Validators
{
    public class ProductCategoryMappingDtoValidator : AbstractValidator<ProductCategoryMappingDto>
    {
        private readonly ILocalizationService _localizationService =
            EngineContext.Current.Resolve<ILocalizationService>();

        private readonly ICategoryService _categoryService = EngineContext.Current.Resolve<ICategoryService>();
        private readonly IProductService _productService = EngineContext.Current.Resolve<IProductService>();


        public ProductCategoryMappingDtoValidator(string httpMethod,
            Dictionary<string, object> passedPropertyValuePaires)
          {
            if (string.IsNullOrEmpty(httpMethod) || httpMethod.Equals("post", StringComparison.InvariantCultureIgnoreCase))
            {
                RuleFor(mapping => mapping.CategoryId)
                    .Must(categoryId => categoryId > 0)
                    .WithMessage(_localizationService.GetResource("Api.ProductCategory.Fields.CategoryId.Invalid"))
                    .DependentRules(mapping =>
                    {
                        mapping.RuleFor(a => a.ProductId)
                            .Must(productId => productId > 0)
                            .WithMessage(_localizationService.GetResource("Api.ProductCategory.Fields.ProductId.Invalid"))
                            .DependentRules(c =>
                            {
                                c.RuleFor(d => d)
                                    .Must(d => _categoryService.GetCategoryById((int)d.CategoryId) != null)
                                    .WithMessage(_localizationService.GetResource("Api.ProductCategory.Fields.Category.Invalid"))
                                    .DependentRules(d =>
                                    {
                                        d.RuleFor(e => e)
                                            .Must(e => _productService.GetProductById((int)e.ProductId) != null)
                                            .WithMessage(_localizationService.GetResource("Api.ProductCategory.Fields.Product.Invalid"))
                                            .DependentRules(f =>
                                            {
                                                f.RuleFor(b => b)
                                                    .Must(b =>
                                                        _categoryService.GetProductCategoriesByCategoryId(
                                                            (int)b.CategoryId, showHidden: true).FindProductCategory((int)b.ProductId, (int)b.CategoryId) == null)
                                                    .WithMessage(_localizationService.GetResource("Api.ProductCategory.Fields.CategoryMapping.AlreadyExist"));
                                            });
                                    });
                            });
                    });
            }
        }
    }
}
