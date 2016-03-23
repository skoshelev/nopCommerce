using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.ActionResults;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Models.CategoriesParameters;
using Nop.Plugin.Api.MVC;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Plugin.Api.Validators;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class CategoriesController : ApiController
    {
        private readonly ICategoryApiService _categoryApiService;
        private readonly IJsonFieldsSerializer _jsonFieldsSerializer;
        private readonly IParametersValidator _parametersValidator;

        public CategoriesController(ICategoryApiService categoryApiService, 
                                    IJsonFieldsSerializer jsonFieldsSerializer, 
                                    IParametersValidator parametersValidator)
        {
            _categoryApiService = categoryApiService;
            _jsonFieldsSerializer = jsonFieldsSerializer;
            _parametersValidator = parametersValidator;
        }

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

            IList<int> idsAsListOfInts = _parametersValidator.GetIdsAsListOfInts(parameters.Ids);

            parameters.PublishedStatus = _parametersValidator.EnsurePublishedStatusIsValid(parameters.PublishedStatus);

            IList<Category> allCategories = _categoryApiService.GetCategories(idsAsListOfInts, parameters.CreatedAtMin, parameters.CreatedAtMax,
                                                                             parameters.UpdatedAtMin, parameters.UpdatedAtMax,
                                                                             parameters.Limit, parameters.Page, parameters.SinceId,
                                                                             parameters.ProductId, parameters.PublishedStatus);

            IList<CategoryDto> categoriesAsDtos = allCategories.Select(x => x.ToDto(parameters.Fields)).ToList();

            var categoriesRootObject = new CategoriesRootObject()
            {
                Categories = categoriesAsDtos
            };

            var json = _jsonFieldsSerializer.Serialize(categoriesRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        [HttpGet]
        [ResponseType(typeof(CategoriesCountRootObject))]
        public IHttpActionResult GetCategoriesCount(CategoriesCountParametersModel parameters)
        {
            parameters.PublishedStatus = _parametersValidator.EnsurePublishedStatusIsValid(parameters.PublishedStatus);

            var allCategoriesCount = _categoryApiService.GetCategoriesCount(parameters.CreatedAtMin, parameters.CreatedAtMax,
                                                                            parameters.UpdatedAtMin, parameters.UpdatedAtMax,
                                                                            parameters.PublishedStatus, parameters.ProductId);

            var categoriesCountRootObject = new CategoriesCountRootObject()
            {
                Count = allCategoriesCount
            };

            return Ok(categoriesCountRootObject);
        }

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
            
            categoriesRootObject.Categories.Add(category.ToDto(fields));
        
            var json = _jsonFieldsSerializer.Serialize(categoriesRootObject, fields);

            return new RawJsonActionResult(json);
        }
    }
}