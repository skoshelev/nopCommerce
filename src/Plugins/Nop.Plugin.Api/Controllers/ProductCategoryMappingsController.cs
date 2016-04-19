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

            var mappingsCount = _productCategoryMappingsService.GetMappingsCount(parameters.ProductId, parameters.CategoryId);

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
    }
}