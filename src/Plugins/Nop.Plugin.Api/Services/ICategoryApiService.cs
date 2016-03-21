using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Services
{
    public interface ICategoryApiService
    {
        IList<Category> GetCategories(IList<int> ids = null, string createdAtMin = "", string createdAtMax = "", string updatedAtMin = "", string updatedAtMax = "",
            byte limit = Configurations.DefaultLimit, int page = 1, int sinceId = 0, int productId = 0, string publishedStatus = Configurations.PublishedStatus);

        int GetCategoriesCount(string createdAtMin, string createdAtMax, string updatedAtMin, string updatedAtMax, string publishedStatus, int productId = 0);
    }
}