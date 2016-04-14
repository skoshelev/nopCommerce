using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.DataStructures;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Services
{
    public class OrderApiService : IOrderApiService
    {
        private readonly IRepository<Order> _orderRepository;

        public OrderApiService(IRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public IList<Order> GetOrdersByCustomerId(int customerId)
        {
            var query = from order in _orderRepository.TableNoTracking
                        where order.CustomerId == customerId
                        orderby order.Id
                        select order;

            return new ApiList<Order>(query, 0, Configurations.MaxLimit);
        }

        public IList<Order> GetOrders(IList<int> ids = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
           int limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int sinceId = Configurations.DefaultSinceId, 
           OrderStatus? status = null, PaymentStatus? financialStatus = null, ShippingStatus? fulfillmentStatus = null, int? customerId = null)
        {
            var query = GetOrdersQuery(createdAtMin, createdAtMax, status, financialStatus, fulfillmentStatus, ids, customerId);

            if (sinceId > 0)
            {
                query = query.Where(order => order.Id > sinceId);
            }

            return new ApiList<Order>(query, page - 1, limit);
        }

        public Order GetOrderById(int orderId)
        {
            if (orderId <= 0)
                return null;

            return _orderRepository.GetById(orderId);
        }

        public int GetOrdersCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null, OrderStatus? status = null,
                                 PaymentStatus? financialStatus = null, ShippingStatus? fulfillmentStatus = null,
                                 int? customerId = null)
        {
            var query = GetOrdersQuery(createdAtMin, createdAtMax, status, financialStatus, fulfillmentStatus, customerId: customerId);

            return query.Count();
        }

        private IQueryable<Order> GetOrdersQuery(DateTime? createdAtMin = null, DateTime? createdAtMax = null, OrderStatus? status = null,
            PaymentStatus? financialStatus = null, ShippingStatus? fulfillmentStatus = null, IList<int> ids = null, 
            int? customerId = null)
        {
            var query = _orderRepository.TableNoTracking;

            if (customerId != null)
            {
                query = query.Where(order => order.CustomerId == customerId);
            }

            if (ids != null && ids.Count > 0)
            {
                query = query.Where(c => ids.Contains(c.Id));
            }
            
            if (status != null)
            {
                query = query.Where(order => order.OrderStatusId == (int)status);
            }
            
            if (financialStatus != null)
            {
                query = query.Where(order => order.PaymentStatusId == (int)financialStatus);
            }
            
            if (fulfillmentStatus != null)
            {
                query = query.Where(order => order.ShippingStatusId == (int)fulfillmentStatus);
            }

            query = query.Where(order => !order.Deleted);

            if (createdAtMin != null)
            {
                query = query.Where(order => order.CreatedOnUtc > createdAtMin.Value.ToUniversalTime());
            }

            if (createdAtMax != null)
            {
                query = query.Where(order => order.CreatedOnUtc < createdAtMax.Value.ToUniversalTime());
            }

            query = query.OrderBy(order => order.Id);

            return query;
        }
    }
}