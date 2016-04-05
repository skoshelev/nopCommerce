using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Services
{
    public interface IProductCategoryMappingsApiService
    {
        IList<ProductCategory> GetMappings(int productId = Configurations.DefaultProductId, int categoryId = Configurations.DefaultCategoryId, 
            int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId);

        int GetMappingsCount(int productId = Configurations.DefaultProductId, int categoryId = Configurations.DefaultCategoryId);

        ProductCategory GetById(int id);
    }
}