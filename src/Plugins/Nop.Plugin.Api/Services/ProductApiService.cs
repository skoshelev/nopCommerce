using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Api.DataStructures;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Services
{
    public class ProductApiService : IProductApiService
    {
        /// <summary>
        /// Key for caching
        /// </summary>
        /// <remarks>
        /// {0} : product ID
        /// </remarks>
        private const string PRODUCTS_BY_ID_KEY = "Nop.product.id-{0}";

        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductCategory> _productCategoryMappingRepository;
        private readonly IRepository<Vendor> _vendorRepository;

        private readonly ICacheManager _cacheManager;

        public ProductApiService(IRepository<Product> productRepository,
            IRepository<ProductCategory> productCategoryMappingRepository,
            IRepository<Vendor> vendorRepository, ICacheManager cacheManager)
        {
            _productRepository = productRepository;
            _productCategoryMappingRepository = productCategoryMappingRepository;
            _vendorRepository = vendorRepository;
            _cacheManager = cacheManager;
        }

        public IList<Product> GetProducts(IList<int> ids = null,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
           int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId,
           int categoryId = 0, string vendorName = "", bool? publishedStatus = null)
        {

            var query = GetProductsQuery(createdAtMin, createdAtMax, updatedAtMin, updatedAtMax, vendorName,
                publishedStatus, ids, categoryId);

            if (sinceId > 0)
            {
                query = query.Where(c => c.Id > sinceId);
            }

            return new ApiList<Product>(query, page - 1, limit);
        }
        
        public int GetProductsCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null, bool? publishedStatus = null, string vendorName = "", int categoryId = 0)
        {
            var query = GetProductsQuery(createdAtMin, createdAtMax, updatedAtMin, updatedAtMax, vendorName,
                                         publishedStatus, categoryId: categoryId);

            return query.Count();
        }

        public Product GetProductById(int productId)
        {
            if (productId == 0)
                return null;

            string key = string.Format(PRODUCTS_BY_ID_KEY, productId);
            return _cacheManager.Get(key, () => _productRepository.GetById(productId));
        }

        private IQueryable<Product> GetProductsQuery(DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
             string vendorName = "", bool? publishedStatus = null, IList<int> ids = null, int categoryId = 0)
            
        {
            var query = _productRepository.Table;

            if (ids != null && ids.Count > 0)
            {
                query = query.Where(c => ids.Contains(c.Id));
            }

            if (publishedStatus != null)
            {
                query = query.Where(c => c.Published == publishedStatus.Value);
            }

            // always return categories that are not deleted!!!
            query = query.Where(c => !c.Deleted);

            if (createdAtMin != null)
            {
                query = query.Where(c => c.CreatedOnUtc > createdAtMin.Value);
            }

            if (createdAtMax != null)
            {
                query = query.Where(c => c.CreatedOnUtc < createdAtMax.Value);
            }

            if (updatedAtMin != null)
            {
               query = query.Where(c => c.UpdatedOnUtc > updatedAtMin.Value);
            }

            if (updatedAtMax != null)
            {
                query = query.Where(c => c.UpdatedOnUtc < updatedAtMax.Value);
            }

            if (!string.IsNullOrEmpty(vendorName))
            {
                query = from vendor in _vendorRepository.Table
                        join product in _productRepository.Table on vendor.Id equals product.VendorId
                        where vendor.Name == vendorName
                        select product;
            }

            //only distinct products (group by ID)
            query = from p in query
                    group p by p.Id
                        into pGroup
                    orderby pGroup.Key
                    select pGroup.FirstOrDefault();

            if (categoryId > 0)
            {
                var categoryMappingsForProduct = from productCategoryMapping in _productCategoryMappingRepository.Table
                                                 where productCategoryMapping.CategoryId == categoryId
                                                 select productCategoryMapping;

                query = from product in query
                        join productCategoryMapping in categoryMappingsForProduct on product.Id equals productCategoryMapping.ProductId
                        select product;
            }

            query = query.OrderBy(product => product.Id);

            return query;
        }
    }
}