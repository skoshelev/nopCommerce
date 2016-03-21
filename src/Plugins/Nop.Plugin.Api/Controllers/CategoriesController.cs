using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Models.CategoriesParameters;
using Nop.Plugin.Api.MVC;
using Nop.Plugin.Api.Services;
using Nop.Services.Catalog;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class CategoriesController : ApiController
    {
        private ICategoryApiService _categoryApiService;
        private ICategoryApiService CategoryApiService
        {
            get
            {
                if (_categoryApiService == null)
                {
                    _categoryApiService = EngineContext.Current.Resolve<ICategoryApiService>();
                }

                return _categoryApiService;
            }
        }

        // nopCommerce's
        private ICategoryService _categoryService;
        private ICategoryService CategoryService
        {
            get
            {
                if (_categoryService == null)
                {
                    _categoryService = EngineContext.Current.Resolve<ICategoryService>();
                }

                return _categoryService;
            }
        }

        [HttpGet]
        [ResponseType(typeof(CategoriesRootObject))]
        public IHttpActionResult GetCategories(CategoriesParametersModel parameters)
        {
            IList<int> idsAsListOfInts = IdsAsListOfInts(parameters.Ids);

            parameters.Limit = EnsureLimitIsValid(parameters.Limit);

            parameters.Page = EnsurePageIsValid(parameters.Page);

            parameters.PublishedStatus = EnsurePublishedStatusIsValid(parameters.PublishedStatus);

            IList<Category> allCategories = CategoryApiService.GetCategories(idsAsListOfInts, parameters.CreatedAtMin, parameters.CreatedAtMax,
                                                                             parameters.UpdatedAtMin, parameters.UpdatedAtMax,
                                                                             parameters.Limit, parameters.Page, parameters.SinceId,
                                                                             parameters.ProductId, parameters.PublishedStatus);

            IList<CategoryDto> categoriesAsDtos = allCategories.Select(x => x.ToDto(parameters.Fields)).ToList();

            var categoriesRootObject = new CategoriesRootObject()
            {
                Categories = categoriesAsDtos
            };

            return Ok(categoriesRootObject);
        }

        [HttpGet]
        [ResponseType(typeof(CategoriesCountRootObject))]
        public IHttpActionResult GetCategoriesCount(CategoriesCountParametersModel parameters)
        {
            parameters.PublishedStatus = EnsurePublishedStatusIsValid(parameters.PublishedStatus);

            var allCategoriesCount = CategoryApiService.GetCategoriesCount(parameters.CreatedAtMin, parameters.CreatedAtMax,
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
                return BadRequest();
            }

            Category category = CategoryService.GetCategoryById(id);

            var categoriesRootObject = new CategoriesRootObject();

            if (category != null)
            {
                categoriesRootObject.Categories.Add(category.ToDto(fields));
            }

            return Ok(categoriesRootObject);
        }

        [NonAction]
        private IList<int> IdsAsListOfInts(string ids)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                return ids.Split(',').Select(int.Parse).ToList();
            }

            return null;
        }

        [NonAction]
        private byte EnsureLimitIsValid(byte limit)
        {
            if (limit <= Configurations.MinLimit || limit > Configurations.MaxLimit)
            {
                limit = Configurations.DefaultLimit;
            }

            return limit;
        }

        [NonAction]
        private int EnsurePageIsValid(int page)
        {
            if (page <= 0)
            {
                page = Configurations.DefaultPageValue;
            }

            return page;
        }

        [NonAction]
        private string EnsurePublishedStatusIsValid(string publishedStatus)
        {
            if (publishedStatus != Configurations.PublishedStatus && publishedStatus != Configurations.UnpublishedStatus &&
                publishedStatus != Configurations.AnyStatus)
            {
                publishedStatus = Configurations.AnyStatus;
            }

            return publishedStatus;
        }
    }
}