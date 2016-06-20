using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.DTOs.Products;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.ProductsParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Seo;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class ProductsController : BaseApiController
    {
        private readonly IProductApiService _productApiService;
        private readonly IProductService _productService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IFactory<Product> _factory; 

        public ProductsController(IProductApiService productApiService, 
                                  IJsonFieldsSerializer jsonFieldsSerializer,
                                  IProductService productService, 
                                  IUrlRecordService urlRecordService, 
                                  ICustomerActivityService customerActivityService, 
                                  ILocalizationService localizationService,
                                  IFactory<Product> factory) : base(jsonFieldsSerializer)
        {
            _productApiService = productApiService;
            _factory = factory;
            _urlRecordService = urlRecordService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _productService = productService;
        }

        /// <summary>
        /// Receive a list of all products
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
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

        /// <summary>
        /// Receive a count of all products
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
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

        /// <summary>
        /// Retrieve product by spcified id
        /// </summary>
        /// <param name="id">Id of the product</param>
        /// <param name="fields">Fields from the product you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(ProductsRootObjectDto))]
        public IHttpActionResult GetProductById(int id, string fields = "")
        {
            if (id <= 0)
            {
                return NotFound();
            }

            Product product = _productApiService.GetProductById(id);

            if (product == null)
            {
                return NotFound();
            }
            
            ProductDto productDto = product.ToDto();

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(productDto);

            var json = _jsonFieldsSerializer.Serialize(productsRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [ResponseType(typeof(ProductsRootObjectDto))]
        public IHttpActionResult CreateProduct([ModelBinder(typeof(JsonModelBinder<ProductDto>))] Delta<ProductDto> productDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }

            //handle pictures.

            // Inserting the new product
            Product newProduct = _factory.Initialize();
            productDelta.Merge(newProduct);

            _productService.InsertProduct(newProduct);

            // Preparing the result dto of the new product
            ProductDto newProductDto = newProduct.ToDto();

            //search engine name
            newProductDto.SeName = newProduct.ValidateSeName(newProductDto.SeName, newProduct.Name, true);
            _urlRecordService.SaveSlug(newProduct, newProductDto.SeName, 0);

            _customerActivityService.InsertActivity("AddNewProduct",
                _localizationService.GetResource("ActivityLog.AddNewProduct"), newProduct.Name);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(newProductDto);

            var json = _jsonFieldsSerializer.Serialize(productsRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }
    }
}