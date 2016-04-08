using System;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Services
{
    public interface IShoppingCartItemApiService
    {
        List<ShoppingCartItem> GetShoppingCartItems(int customerId = Configurations.DefaultCustomerId, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
                                                    DateTime? updatedAtMin = null, DateTime? updatedAtMax = null, int limit = Configurations.DefaultLimit, 
                                                    int page = Configurations.DefaultPageValue);
    }
}