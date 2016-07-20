﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTOs.ProductCategoryMappings;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.ProductCategoryMappingsParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class ProductCategoryMappingsController : BaseApiController
    {
        private readonly IProductCategoryMappingsApiService _productCategoryMappingsService;
        private readonly ICategoryService _categoryService;
        private readonly ICategoryApiService _categoryApiService;
        private readonly IProductApiService _productApiService;

        public ProductCategoryMappingsController(IProductCategoryMappingsApiService productCategoryMappingsService,
            ICategoryService categoryService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService, 
            ICategoryApiService categoryApiService, 
            IProductApiService productApiService)
            : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService)
        {
            _productCategoryMappingsService = productCategoryMappingsService;
            _categoryService = categoryService;
            _categoryApiService = categoryApiService;
            _productApiService = productApiService;
        }

        /// <summary>
        /// Receive a list of all Product-Category mappings
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(ProductCategoryMappingsRootObject))]
        public IHttpActionResult GetMappings(ProductCategoryMappingsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return BadRequest("Invalid request parameters");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return BadRequest("Invalid request parameters");
            }

            IList<ProductCategoryMappingDto> mappingsAsDtos =
                _productCategoryMappingsService.GetMappings(parameters.ProductId,
                    parameters.CategoryId,
                    parameters.Limit,
                    parameters.Page,
                    parameters.SinceId).Select(x => x.ToDto()).ToList();

            var productCategoryMappingRootObject = new ProductCategoryMappingsRootObject()
            {
                ProductCategoryMappingDtos = mappingsAsDtos
            };

            var json = _jsonFieldsSerializer.Serialize(productCategoryMappingRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Receive a count of all Product-Category mappings
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(ProductCategoryMappingsCountRootObject))]
        public IHttpActionResult GetMappingsCount(ProductCategoryMappingsCountParametersModel parameters)
        {
            if (parameters.ProductId < 0 || parameters.CategoryId < 0)
            {
                return BadRequest("Invalid request parameters");
            }

            var mappingsCount = _productCategoryMappingsService.GetMappingsCount(parameters.ProductId,
                parameters.CategoryId);

            var productCategoryMappingsCountRootObject = new ProductCategoryMappingsCountRootObject()
            {
                Count = mappingsCount
            };

            return Ok(productCategoryMappingsCountRootObject);
        }

        /// <summary>
        /// Retrieve Product-Category mappings by spcified id
        /// </summary>
        ///   /// <param name="id">Id of the Product-Category mapping</param>
        /// <param name="fields">Fields from the Product-Category mapping you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(ProductCategoryMappingsRootObject))]
        public IHttpActionResult GetMappingById(int id, string fields = "")
        {
            if (id <= 0)
            {
                return NotFound();
            }

            ProductCategory mapping = _productCategoryMappingsService.GetById(id);

            if (mapping == null)
            {
                return NotFound();
            }

            var productCategoryMappingsRootObject = new ProductCategoryMappingsRootObject();
            productCategoryMappingsRootObject.ProductCategoryMappingDtos.Add(mapping.ToDto());

            var json = _jsonFieldsSerializer.Serialize(productCategoryMappingsRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [ResponseType(typeof(ProductCategoryMappingsRootObject))]
        public IHttpActionResult CreateProductCategoryMapping([ModelBinder(typeof(JsonModelBinder<ProductCategoryMappingDto>))] Delta<ProductCategoryMappingDto> productCategoryDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            Category category = _categoryApiService.GetCategoryById(productCategoryDelta.Dto.CategoryId.Value);
            if (category == null)
            {
                ModelState.AddModelError("category_id", "Invalid");

                return Error();
            }

            Product product = _productApiService.GetProductById(productCategoryDelta.Dto.ProductId.Value);
            if (product == null)
            {
                ModelState.AddModelError("product_id", "Invalid");

                return Error();
            }

            ProductCategory newProductCategory = new ProductCategory();
            productCategoryDelta.Merge(newProductCategory);

            //inserting new category
            _categoryService.InsertProductCategory(newProductCategory);

            // Preparing the result dto of the new product category mapping
            ProductCategoryMappingDto newProductCategoryMappingDto = newProductCategory.ToDto();

            var productCategoryMappingsRootObject = new ProductCategoryMappingsRootObject();

            productCategoryMappingsRootObject.ProductCategoryMappingDtos.Add(newProductCategoryMappingDto);

            var json = _jsonFieldsSerializer.Serialize(productCategoryMappingsRootObject, string.Empty);

            //activity log 
            _customerActivityService.InsertActivity("AddNewProductCategoryMapping", _localizationService.GetResource("ActivityLog.AddNewProductCategoryMapping"), newProductCategory.Id);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [ResponseType(typeof(ProductCategoryMappingsRootObject))]
        public IHttpActionResult UpdateProductCategoryMapping([ModelBinder(typeof(JsonModelBinder<ProductCategoryMappingDto>))] Delta<ProductCategoryMappingDto> productCategoryDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            if (productCategoryDelta.Dto.CategoryId.HasValue)
            {
                Category category = _categoryApiService.GetCategoryById(productCategoryDelta.Dto.CategoryId.Value);
                if (category == null)
                {
                    ModelState.AddModelError("category_id", "Invalid");

                    return Error();
                }
            }

            if (productCategoryDelta.Dto.ProductId.HasValue)
            {
                Product product = _productApiService.GetProductById(productCategoryDelta.Dto.ProductId.Value);
                if (product == null)
                {
                    ModelState.AddModelError("product_id", "Invalid");

                    return Error();
                }
            }

            // We do not need to validate the category id, because this will happen in the model binder using the dto validator.
            int updateProductCategoryId = productCategoryDelta.Dto.Id;

            ProductCategory productCategoryEntityToUpdate = _categoryService.GetProductCategoryById(updateProductCategoryId);

            if (productCategoryEntityToUpdate == null)
            {
                ModelState.AddModelError("product_category_mapping", "Not Found");
                return Error();
            }

            productCategoryDelta.Merge(productCategoryEntityToUpdate);

            _categoryService.UpdateProductCategory(productCategoryEntityToUpdate);

            //activity log
            _customerActivityService.InsertActivity("UpdateProdutCategoryMapping",
                _localizationService.GetResource("ActivityLog.UpdateProdutCategoryMapping"), productCategoryEntityToUpdate.Id);

            ProductCategoryMappingDto updatedProductCategoryDto = productCategoryEntityToUpdate.ToDto();

            var productCategoriesRootObject = new ProductCategoryMappingsRootObject();

            productCategoriesRootObject.ProductCategoryMappingDtos.Add(updatedProductCategoryDto);

            var json = _jsonFieldsSerializer.Serialize(productCategoriesRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        public IHttpActionResult DeleteProductCategoryMapping(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            ProductCategory productCategory = _categoryService.GetProductCategoryById(id);

            if (productCategory == null)
            {
                return NotFound();
            }

            _categoryService.DeleteProductCategory(productCategory);

            //activity log 
            _customerActivityService.InsertActivity("DeleteProductCategoryMapping", _localizationService.GetResource("ActivityLog.DeleteProductCategoryMapping"), productCategory.Id);

            return new RawJsonActionResult("{}");
        }
    }
}