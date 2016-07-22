using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.DTOs.OrderItems;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Models.OrderItemsParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class OrderItemsController : BaseApiController
    {
        private readonly IOrderItemApiService _orderItemApiService;
        private readonly IOrderApiService _orderApiService;
        private readonly IOrderService _orderService;

        public OrderItemsController(IJsonFieldsSerializer jsonFieldsSerializer, 
            IAclService aclService, 
            ICustomerService customerService, 
            IStoreMappingService storeMappingService, 
            IStoreService storeService, 
            IDiscountService discountService, 
            ICustomerActivityService customerActivityService, 
            ILocalizationService localizationService, 
            IOrderItemApiService orderItemApiService, 
            IOrderApiService orderApiService, 
            IOrderService orderService) 
            : base(jsonFieldsSerializer, 
                  aclService, 
                  customerService, 
                  storeMappingService, 
                  storeService, 
                  discountService, 
                  customerActivityService, 
                  localizationService)
        {
            _orderItemApiService = orderItemApiService;
            _orderApiService = orderApiService;
            _orderService = orderService;
        }
        
        [HttpGet]
        [ResponseType(typeof(OrderItemsRootObject))]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult GetOrderItems(int orderId, OrderItemsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "Invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "Invalid request parameters");
            }

            Order order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            IList<OrderItem> allOrderItemsForOrder = _orderItemApiService.GetOrderItemsForOrder(order, parameters.Limit, parameters.Page, parameters.SinceId);

            var orderItemsRootObject = new OrderItemsRootObject()
            {
                OrderItems = allOrderItemsForOrder.Select(item => item.ToDto()).ToList()
            };

            var json = _jsonFieldsSerializer.Serialize(orderItemsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        [HttpGet]
        [ResponseType(typeof(OrderItemsCountRootObject))]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult GetOrderItemsCount(int orderId)
        {
            Order order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }
            
            int orderItemsCountForOrder = _orderItemApiService.GetOrderItemsCount(order);

            var orderItemsCountRootObject = new OrderItemsCountRootObject()
            {
                Count = orderItemsCountForOrder
            };

            return Ok(orderItemsCountRootObject);
        }

        [HttpGet]
        [ResponseType(typeof(OrderItemsRootObject))]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult GetOrderItemByIdForOrder(int orderId, int orderItemId, string fields = "")
        {
            Order order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            OrderItem orderItem = _orderService.GetOrderItemById(orderItemId);

            if (orderItem == null)
            {
                return Error(HttpStatusCode.NotFound, "order_item", "not found");
            }

            var orderItemDtos = new List<OrderItemDto>();
            orderItemDtos.Add(orderItem.ToDto());

            var orderItemsRootObject = new OrderItemsRootObject()
            {
                OrderItems = orderItemDtos
            };

            var json = _jsonFieldsSerializer.Serialize(orderItemsRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpDelete]
        [GetRequestsErrorInterceptorActionFilter]
        public IHttpActionResult DeleteOrderItemById(int orderId, int orderItemId)
        {
            Order order = _orderApiService.GetOrderById(orderId);

            if (order == null)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            OrderItem orderItem = _orderService.GetOrderItemById(orderItemId);
            _orderService.DeleteOrderItem(orderItem);
            
            return new RawJsonActionResult("{}");
        }
    }
}