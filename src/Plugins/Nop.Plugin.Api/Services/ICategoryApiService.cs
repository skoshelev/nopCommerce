﻿using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Services
{
    public interface ICategoryApiService
    {
        Category GetCategoryById(int categoryId);

        IList<Category> GetCategories(IList<int> ids = null,
            DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            int limit = Configurations.DefaultLimit, int page = 1, int sinceId = 0, int productId = 0, 
            bool? publishedStatus = null);

        int GetCategoriesCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null, DateTime? updatedAtMin = null, DateTime? updatedAtMax = null,
            bool? publishedStatus = null, int productId = 0);
    }
}