using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Models.CategoriesParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Seo;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTOs.Errors;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.ModelBinders;

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
        private readonly IFactory<Category> _factory; 

        public CategoriesController(ICategoryApiService categoryApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            ICategoryService categoryService,
            IUrlRecordService urlRecordService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService, 
            IFactory<Category> factory)
        {
            _categoryApiService = categoryApiService;
            _jsonFieldsSerializer = jsonFieldsSerializer;
            _categoryService = categoryService;
            _urlRecordService = urlRecordService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _factory = factory;
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
        public IHttpActionResult CreateCategory([ModelBinder(typeof(JsonModelBinder<CategoryDto>))] Delta<CategoryDto> categoryDelta)
        {
            // Validation
            if (!ModelState.IsValid)
            {
                return Error();
            }

            // Inserting the new category
            Category newCategory = _factory.Initialize();
            categoryDelta.Merge(newCategory);

            _categoryService.InsertCategory(newCategory);

            // TODO: Localization
            // TODO: Pictures

            // Preparing the result dto of the new category
            CategoryDto newCategoryDto = newCategory.ToDto();

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

        private IHttpActionResult Error()
        {
            var errors = new Dictionary<string, List<string>>();

            foreach (var item in ModelState)
            {
                var errorMessages = item.Value.Errors.Select(x => x.ErrorMessage);

                if (errors.ContainsKey(item.Key))
                {
                    errors[item.Key].AddRange(errorMessages);
                }
                else
                {
                    errors.Add(item.Key, errorMessages.ToList());
                }
            }

            var errorsRootObject = new ErrorsRootObject()
            {
                Errors = errors
            };

            var errorsJson = _jsonFieldsSerializer.Serialize(errorsRootObject, null);

            return new ErrorActionResult(errorsJson);
        }
    }
}