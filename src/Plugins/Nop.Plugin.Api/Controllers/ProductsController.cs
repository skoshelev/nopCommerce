using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.ActionResults;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTOs.Products;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Models.ProductsParameters;
using Nop.Plugin.Api.MVC;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Plugin.Api.Validators;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class ProductsController : ApiController
    {
        private readonly IProductApiService _productApiService;
        private readonly IJsonFieldsSerializer _jsonFieldsSerializer;

        public ProductsController(IProductApiService productApiService, 
                                  IJsonFieldsSerializer jsonFieldsSerializer)
        {
            _productApiService = productApiService;
            _jsonFieldsSerializer = jsonFieldsSerializer;
        }

        [HttpGet]
        [ResponseType(typeof(ProductsRootObjectDto))]
        public IHttpActionResult GetProducts(ProductsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return BadRequest("Invalid request parameters");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return BadRequest("Invalid request parameters");
            }

            IList<Product> allProducts = _productApiService.GetProducts(parameters.Ids, parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.UpdatedAtMin,
                                                                        parameters.UpdatedAtMax, parameters.Limit, parameters.Page, parameters.SinceId, parameters.CategoryId,
                                                                        parameters.VendorName, parameters.PublishedStatus);

            IList<ProductDto> productsAsDtos = allProducts.Select(x => x.ToDto()).ToList();

            var productsRootObject = new ProductsRootObjectDto()
            {
                Products = productsAsDtos
            };

            var json = _jsonFieldsSerializer.Serialize(productsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        [HttpGet]
        [ResponseType(typeof(ProductsCountRootObject))]
        public IHttpActionResult GetProductsCount(ProductsCountParametersModel parameters)
        {
            var allProductsCount = _productApiService.GetProductsCount(parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.UpdatedAtMin,
                                                                       parameters.UpdatedAtMax, parameters.PublishedStatus, parameters.VendorName, 
                                                                       parameters.CategoryId);

            var productsCountRootObject = new ProductsCountRootObject()
            {
                Count = allProductsCount
            };

            return Ok(productsCountRootObject);
        }

        [HttpGet]
        [ResponseType(typeof(ProductsRootObjectDto))]
        public IHttpActionResult GetProductById(int id, string fields = "")
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            Product product = _productApiService.GetProductById(id);

            var productsRootObject = new ProductsRootObjectDto();

            if (product != null)
            {
                ProductDto productDto = product.ToDto(fields);

                productsRootObject.Products.Add(productDto);
            }

            var json = _jsonFieldsSerializer.Serialize(productsRootObject, fields);

            return new RawJsonActionResult(json);
        }
    }
}