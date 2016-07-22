using System;
using System.Collections.Generic;
using FluentValidation;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.DTOs.ProductCategoryMappings;
using Nop.Services.Catalog;

namespace Nop.Plugin.Api.Validators
{
    public class ProductCategoryMappingDtoValidator : AbstractValidator<ProductCategoryMappingDto>
    {
        private readonly ICategoryService _categoryService = EngineContext.Current.Resolve<ICategoryService>();
        private readonly IProductService _productService = EngineContext.Current.Resolve<IProductService>();


        public ProductCategoryMappingDtoValidator(string httpMethod, Dictionary<string, object> passedPropertyValuePaires)
        {
            if (string.IsNullOrEmpty(httpMethod) || httpMethod.Equals("post", StringComparison.InvariantCultureIgnoreCase))
            {
                RuleFor(mapping => mapping.CategoryId)
                    .Must(categoryId => categoryId > 0)
                    .WithMessage("invalid category_id")
                    .DependentRules(mapping =>
                    {
                        mapping.RuleFor(a => a.ProductId)
                            .Must(productId => productId > 0)
                            .WithMessage("invalid product_id")
                            .DependentRules(c =>
                            {
                                c.RuleFor(d => d)
                                    .Must(d => _categoryService.GetCategoryById((int)d.CategoryId) != null)
                                    .WithMessage("category does not exist")
                                    .DependentRules(d =>
                                    {
                                        d.RuleFor(e => e)
                                            .Must(e => _productService.GetProductById((int)e.ProductId) != null)
                                            .WithMessage("product does not exist")
                                            .DependentRules(f =>
                                            {
                                                f.RuleFor(b => b)
                                                    .Must(b =>
                                                        _categoryService.GetProductCategoriesByCategoryId(
                                                            (int)b.CategoryId, showHidden: true).FindProductCategory((int)b.ProductId, (int)b.CategoryId) == null)
                                                    .WithMessage("mapping already exist");
                                            });
                                    });
                            });
                    });
            }
            else if (httpMethod.Equals("put", StringComparison.InvariantCultureIgnoreCase))
            {
                RuleFor(mapping => mapping.Id)
                    .NotNull()
                    .NotEmpty()
                    .Must(id => id > 0)
                    .WithMessage("invalid id");

                if (passedPropertyValuePaires.ContainsKey("category_id"))
                {
                    RuleFor(mapping => mapping.CategoryId)
                        .Must(categoryId => categoryId > 0)
                        .WithMessage("category_id invalid")
                        .DependentRules(mapping =>
                        {
                            mapping.RuleFor(m => m.CategoryId)
                                .Must(categoryId => _categoryService.GetCategoryById((int)categoryId) != null)
                                .WithMessage("category does not exist");
                        });
                }

                if (passedPropertyValuePaires.ContainsKey("product_id"))
                {
                    RuleFor(mapping => mapping.ProductId)
                        .Must(productId => productId > 0)
                        .WithMessage("product_id invalid")
                        .DependentRules(mapping =>
                         {
                             mapping.RuleFor(m => m.ProductId)
                                    .Must(productId => _productService.GetProductById((int)productId) != null)
                                    .WithMessage("product does not exist");
                         });
                }
            }
        }
    }
}
