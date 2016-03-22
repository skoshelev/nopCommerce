using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DataStructures;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Services
{
    public class CategoryApiService : ICategoryApiService
    {
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<ProductCategory> _productCategoryMappingRepository;

        public CategoryApiService(IRepository<Category> categoryRepository,
            IRepository<ProductCategory> productCategoryMappingRepository)
        {
            _categoryRepository = categoryRepository;
            _productCategoryMappingRepository = productCategoryMappingRepository;
        }

        public IList<Category> GetCategories(IList<int> ids = null, string createdAtMin = "", string createdAtMax = "", string updatedAtMin = "",
             string updatedAtMax = "", int limit = Configurations.DefaultLimit, int page = 1, int sinceId = 0, int productId = 0, string publishedStatus = Configurations.PublishedStatus)
        {

            var query = GetCategoriesQuery(createdAtMin, createdAtMax, updatedAtMin, updatedAtMax,
                publishedStatus, ids, productId);

            if (sinceId > 0)
            {
                query = query.Where(c => c.Id > sinceId);
            }

            return new ApiList<Category>(query, page - 1, limit);
        }

        public Category GetCategoryById(int id)
        {
            if (id == 0)
                return null;

            Category category = _categoryRepository.GetById(id);

            return category;
        }

        public int GetCategoriesCount(string createdAtMin = "", string createdAtMax = "", string updatedAtMin = "", string updatedAtMax = "",
            string publishedStatus = "", int productId = 0)
        {
            var query = GetCategoriesQuery(createdAtMin, createdAtMax, updatedAtMin, updatedAtMax,
                                           publishedStatus, productId: productId);

            return query.Count();
        }

        private IQueryable<Category> GetCategoriesQuery(string createdAtMin = "", string createdAtMax = "", string updatedAtMin = "",
             string updatedAtMax = "", string publishedStatus = Configurations.PublishedStatus, IList<int> ids = null, int productId = 0)
        {
            var query = _categoryRepository.Table;

            if (ids != null && ids.Count > 0)
            {
                query = query.Where(c => ids.Contains(c.Id));
            }

            if (publishedStatus == Configurations.PublishedStatus)
            {
                query = query.Where(c => c.Published);
            }
            else if (publishedStatus == Configurations.UnpublishedStatus)
            {
                query = query.Where(c => !c.Published);
            }

            query = query.Where(c => !c.Deleted);

            if (!string.IsNullOrEmpty(createdAtMin))
            {
                var createAtMin = DateTime.Parse(createdAtMin).ToUniversalTime();
                query = query.Where(c => c.CreatedOnUtc > createAtMin);
            }

            if (!string.IsNullOrEmpty(createdAtMax))
            {
                var createAtMax = DateTime.Parse(createdAtMax).ToUniversalTime();
                query = query.Where(c => c.CreatedOnUtc < createAtMax);
            }

            if (!string.IsNullOrEmpty(updatedAtMin))
            {
                var updatedAtMinAsDateTime = DateTime.Parse(updatedAtMin).ToUniversalTime();
                query = query.Where(c => c.UpdatedOnUtc > updatedAtMinAsDateTime);
            }

            if (!string.IsNullOrEmpty(updatedAtMax))
            {
                var updatedAtMaxAsDateTime = DateTime.Parse(updatedAtMax).ToUniversalTime();
                query = query.Where(c => c.UpdatedOnUtc < updatedAtMaxAsDateTime);
            }

            //only distinct categories (group by ID)
            query = from c in query
                    group c by c.Id
                        into cGroup
                    orderby cGroup.Key
                    select cGroup.FirstOrDefault();

            if (productId > 0)
            {
                var categoryMappingsForProduct = from productCategoryMapping in _productCategoryMappingRepository.Table
                                                 where productCategoryMapping.ProductId == productId
                                                 select productCategoryMapping;

                query = from category in query
                        join productCategoryMapping in categoryMappingsForProduct on category.Id equals productCategoryMapping.CategoryId
                        select category;
            }

            query = query.OrderBy(category => category.Id);

            return query;
        }
    }
}