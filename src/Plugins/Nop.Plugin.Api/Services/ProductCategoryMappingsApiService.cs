using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.DataStructures;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Services
{
    public class ProductCategoryMappingsApiService : IProductCategoryMappingsApiService
    {
        private readonly IRepository<ProductCategory> _productCategoryMappingsRepository;

        public ProductCategoryMappingsApiService(IRepository<ProductCategory> productCategoryMappingsRepository)
        {
            _productCategoryMappingsRepository = productCategoryMappingsRepository;
        }

        public IList<ProductCategory> GetMappings(int productId = Configurations.DefaultProductId, 
            int categoryId = Configurations.DefaultCategoryId, int limit = Configurations.DefaultLimit, 
            int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId)
        {
            var query = GetMappingsQuery(productId, categoryId, sinceId);

            return new ApiList<ProductCategory>(query, page - 1, limit);
        }

        public int GetMappingsCount(int productId = Configurations.DefaultProductId, int categoryId = Configurations.DefaultCategoryId)
        {
            return GetMappingsQuery(productId, categoryId).Count();
        }

        public ProductCategory GetById(int id)
        {
            if (id <= 0)
                return null;

            return _productCategoryMappingsRepository.GetById(id);
        }

        private IQueryable<ProductCategory> GetMappingsQuery(int productId = Configurations.DefaultProductId, 
            int categoryId = Configurations.DefaultCategoryId, int sinceId = Configurations.DefaultSinceId)
        {
            var query = _productCategoryMappingsRepository.Table;

            if (productId > 0)
            {
                query = query.Where(mapping => mapping.ProductId == productId);
            }

            if (categoryId > 0)
            {
                query = query.Where(mapping => mapping.CategoryId == categoryId);
            }

            if (sinceId > 0)
            {
                query = query.Where(mapping => mapping.Id > sinceId);
            }

            query = query.OrderBy(mapping => mapping.Id);

            return query;
        }
    }
}