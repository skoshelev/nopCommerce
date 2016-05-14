using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.ActionResults;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.CategoriesParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Seo;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class CategoriesController : ApiController
    {
        private readonly ICategoryApiService _categoryApiService;
        private readonly ICategoryService _categoryService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IJsonFieldsSerializer _jsonFieldsSerializer;
        private readonly IMappingHelper _mappingHelper;

        public CategoriesController(ICategoryApiService categoryApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            ICategoryService categoryService,
            IUrlRecordService urlRecordService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IMappingHelper mappingHelper)
        {
            _categoryApiService = categoryApiService;
            _jsonFieldsSerializer = jsonFieldsSerializer;
            _categoryService = categoryService;
            _urlRecordService = urlRecordService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _mappingHelper = mappingHelper;
        }

        /// <summary>
        /// Receive a list of all Categories
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(CategoriesRootObject))]
        public IHttpActionResult GetCategories(CategoriesParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return BadRequest("Invalid request parameters");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return BadRequest("Invalid request parameters");
            }

            IList<Category> allCategories = _categoryApiService.GetCategories(parameters.Ids, parameters.CreatedAtMin, parameters.CreatedAtMax,
                                                                             parameters.UpdatedAtMin, parameters.UpdatedAtMax,
                                                                             parameters.Limit, parameters.Page, parameters.SinceId,
                                                                             parameters.ProductId, parameters.PublishedStatus);

            IList<CategoryDto> categoriesAsDtos = allCategories.Select(x => x.ToDto()).ToList();

            var categoriesRootObject = new CategoriesRootObject()
            {
                Categories = categoriesAsDtos
            };

            var json = _jsonFieldsSerializer.Serialize(categoriesRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Receive a count of all Categories
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(CategoriesCountRootObject))]
        public IHttpActionResult GetCategoriesCount(CategoriesCountParametersModel parameters)
        {
            var allCategoriesCount = _categoryApiService.GetCategoriesCount(parameters.CreatedAtMin, parameters.CreatedAtMax,
                                                                            parameters.UpdatedAtMin, parameters.UpdatedAtMax,
                                                                            parameters.PublishedStatus, parameters.ProductId);

            var categoriesCountRootObject = new CategoriesCountRootObject()
            {
                Count = allCategoriesCount
            };

            return Ok(categoriesCountRootObject);
        }

        /// <summary>
        /// Retrieve category by spcified id
        /// </summary>
        /// <param name="id">Id of the category</param>
        /// <param name="fields">Fields from the category you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(CategoriesRootObject))]
        public IHttpActionResult GetCategoryById(int id, string fields = "")
        {
            if (id <= 0)
            {
                return NotFound();
            }

            Category category = _categoryApiService.GetCategoryById(id);

            if (category == null)
            {
                return NotFound();
            }

            var categoriesRootObject = new CategoriesRootObject();

            categoriesRootObject.Categories.Add(category.ToDto());

            var json = _jsonFieldsSerializer.Serialize(categoriesRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [ResponseType(typeof(CategoriesRootObject))]
        // Here we use JsonModelBinder so we don't have to create a special binder for the DTO objects and couple the DTO objects with it,
        // which will make them a kind of parameter object which they are not. 
        public IHttpActionResult CreateCategory([ModelBinder(typeof(JsonModelBinder))] Dictionary<string, object> categoryRoot)
        {
            if (categoryRoot == null || string.IsNullOrEmpty(categoryRoot.ToString()))
            {
                return BadRequest("Invalid category passed");
            }

            if (!categoryRoot.ContainsKey("category"))
            {
                return BadRequest("Invalid category passed");
            }


            var newCategoryDto = new CategoryDto();

            Dictionary<string, object> categoryProperties = (Dictionary<string, object>)categoryRoot["category"];

            _mappingHelper.SetValues(categoryProperties, newCategoryDto, typeof(CategoryDto));

            Category newCategory = newCategoryDto.ToEntity();
            newCategory.CreatedOnUtc = DateTime.UtcNow;
            newCategory.UpdatedOnUtc = DateTime.UtcNow;
            _categoryService.InsertCategory(newCategory);

            // TODO: Localization
            // TODO: Discounts
            // TODO: Pictures
            // TODO: ACL
            // TODO: StoreMappings

            //search engine name
            newCategoryDto.SeName = newCategory.ValidateSeName(newCategoryDto.SeName, newCategory.Name, true);
            _urlRecordService.SaveSlug(newCategory, newCategoryDto.SeName, 0);

            _customerActivityService.InsertActivity("AddNewCategory",
                _localizationService.GetResource("ActivityLog.AddNewCategory"), newCategory.Name);

            var categoriesRootObject = new CategoriesRootObject();

            categoriesRootObject.Categories.Add(newCategoryDto);

            var json = _jsonFieldsSerializer.Serialize(categoriesRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }
    }
}