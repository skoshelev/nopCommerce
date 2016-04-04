using System;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Services
{
    public interface IOrderApiService
    {
        IList<Order> GetOrdersByCustomerId(int customerId);

        IList<Order> GetOrders(IList<int> ids = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
                               int limit = Configurations.DefaultLimit, int page = 1, int sinceId = 0, OrderStatus? status = null,
                               PaymentStatus? financialStatus = null, ShippingStatus? fulfillmentStatus = null, int? customerId = null);

        Order GetOrderById(int orderId);

        int GetOrdersCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null, OrderStatus? status = null,
                           PaymentStatus? financialStatus = null, ShippingStatus? fulfillmentStatus = null,
                           int? customerId = null);
    }
}