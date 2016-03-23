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

        public IList<Product> GetProducts(IList<int> ids = null, string createdAtMin = "", string createdAtMax = "", string updatedAtMin = "",
             string updatedAtMax = "", int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, 
             int sinceId = Configurations.DefaultSinceId, int categoryId = 0, string vendorName = "", string publishedStatus = Configurations.PublishedStatus)
        {

            var query = GetProductsQuery(createdAtMin, createdAtMax, updatedAtMin, updatedAtMax, vendorName,
                publishedStatus, ids, categoryId);

            if (sinceId > 0)
            {
                query = query.Where(c => c.Id > sinceId);
            }

            return new ApiList<Product>(query, page - 1, limit);
        }
        
        public int GetProductsCount(string createdAtMin, string createdAtMax, string updatedAtMin, string updatedAtMax,
            string publishedStatus, string vendorName = "", int categoryId = 0)
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

        private IQueryable<Product> GetProductsQuery(string createdAtMin = "", string createdAtMax = "", string updatedAtMin = "",
             string updatedAtMax = "", string vendorName = "", string publishedStatus = Configurations.PublishedStatus, IList<int> ids = null, int categoryId = 0)
        {
            var query = _productRepository.Table;

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