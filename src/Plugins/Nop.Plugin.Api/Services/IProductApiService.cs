using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Services
{
    public interface IProductApiService
    {
        IList<Product> GetProducts(IList<int> ids = null, string createdAtMin = "", string createdAtMax = "", string updatedAtMin = "", string updatedAtMax = "",
           int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId, 
           int categoryId = 0, string vendorName = "", string publishedStatus = Configurations.PublishedStatus);

        int GetProductsCount(string createdAtMin, string createdAtMax, string updatedAtMin, string updatedAtMax, string publishedStatus, string vendorName, int categoryId = 0);

        Product GetProductById(int productId);
    }
}