using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.ActionResults;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTOs.ProductCategoryMappings;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Models.ProductCategoryMappingsParameters;
using Nop.Plugin.Api.MVC;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class ProductCategoryMappingsController : ApiController
    {
        private readonly IProductCategoryMappingsApiService _productCategoryMappingsService;
        private readonly IJsonFieldsSerializer _jsonFieldsSerializer;

        public ProductCategoryMappingsController(IProductCategoryMappingsApiService productCategoryMappingsService, IJsonFieldsSerializer jsonFieldsSerializer)
        {
            _productCategoryMappingsService = productCategoryMappingsService;
            _jsonFieldsSerializer = jsonFieldsSerializer;
        }

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

            IList<ProductCategoryMappingDto> mappingsAsDtos = _productCategoryMappingsService.GetMappings(parameters.ProductId, 
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

        [HttpGet]
        [ResponseType(typeof(ProductCategoryMappingsCountRootObject))]
        public IHttpActionResult GetMappingsCount(ProductCategoryMappingsCountParametersModel parameters)
        {
            if (parameters.ProductId < 0 || parameters.CategoryId < 0)
            {
                return BadRequest("Invalid request parameters");
            }

            var mappingsCount = _productCategoryMappingsService.GetMappingsCount(parameters.ProductId, parameters.CategoryId);

            var productCategoryMappingsCountRootObject = new ProductCategoryMappingsCountRootObject()
            {
                Count = mappingsCount
            };

            return Ok(productCategoryMappingsCountRootObject);
        }

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
    }
}