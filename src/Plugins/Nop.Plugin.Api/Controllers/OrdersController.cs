using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.ActionResults;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTOs.Orders;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Models.OrdersParameters;
using Nop.Plugin.Api.MVC;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class OrdersController : ApiController
    {
        private readonly IOrderApiService _orderApiService;
        private readonly IJsonFieldsSerializer _jsonFieldsSerializer;

        public OrdersController(IOrderApiService orderApiService, IJsonFieldsSerializer jsonFieldsSerializer)
        {
            _orderApiService = orderApiService;
            _jsonFieldsSerializer = jsonFieldsSerializer;
        }

        [HttpGet]
        [ResponseType(typeof(OrdersRootObject))]
        public IHttpActionResult GetOrders(OrdersParametersModel parameters)
        {
            if (parameters.Page <= 0)
            {
                return BadRequest("Invalid request parameters");
            }

            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return BadRequest("Invalid request parameters");
            }

            IList<OrderDto> ordersAsDtos = _orderApiService.GetOrders(parameters.Ids, parameters.CreatedAtMin, parameters.CreatedAtMax,
                                                                      parameters.Limit, parameters.Page, parameters.SinceId, 
                                                                      parameters.Status, parameters.PaymentStatus, parameters.ShippingStatus,
                                                                      parameters.CustomerId).Select(x => x.ToDto()).ToList();

            var ordersRootObject = new OrdersRootObject()
            {
                Orders = ordersAsDtos
            };

            var json = _jsonFieldsSerializer.Serialize(ordersRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        [HttpGet]
        [ResponseType(typeof(OrdersCountRootObject))]
        public IHttpActionResult GetOrdersCount(OrdersCountParametersModel parameters)
        {
            var ordersCount = _orderApiService.GetOrdersCount(parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.Status,
                                                              parameters.PaymentStatus, parameters.ShippingStatus, parameters.CustomerId);

            var ordersCountRootObject = new OrdersCountRootObject()
            {
                Count = ordersCount
            };

            return Ok(ordersCountRootObject);
        }

        [HttpGet]
        [ResponseType(typeof(OrdersRootObject))]
        public IHttpActionResult GetOrderById(int id, string fields = "")
        {
            if (id <= 0)
            {
                return NotFound()   ;
            }

            Order order = _orderApiService.GetOrderById(id);

            if (order == null)
            {
                return NotFound();
            }

            var ordersRootObject = new OrdersRootObject();

            OrderDto orderDto = order.ToDto();
            ordersRootObject.Orders.Add(orderDto);

            var json = _jsonFieldsSerializer.Serialize(ordersRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpGet]
        [ResponseType(typeof(OrdersRootObject))]
        public IHttpActionResult GetOrdersByCustomerId(int customer_id)
        {
            IList<OrderDto> ordersForCustomer = _orderApiService.GetOrdersByCustomerId(customer_id).Select(x => x.ToDto()).ToList();

            var ordersRootObject = new OrdersRootObject()
            {
                Orders = ordersForCustomer
            };

            return Ok(ordersRootObject);
        }
    }
}