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

        /// <summary>
        /// Receive a list of all Orders
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
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

        /// <summary>
        /// Receive a count of all Orders
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
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

        /// <summary>
        /// Retrieve order by spcified id
        /// </summary>
        ///   /// <param name="id">Id of the order</param>
        /// <param name="fields">Fields from the order you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
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

        /// <summary>
        /// Retrieve all orders for customer
        /// </summary>
        /// <param name="customerId">Id of the customer whoes orders you want to get</param>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(OrdersRootObject))]
        public IHttpActionResult GetOrdersByCustomerId(int customerId)
        {
            IList<OrderDto> ordersForCustomer = _orderApiService.GetOrdersByCustomerId(customerId).Select(x => x.ToDto()).ToList();

            var ordersRootObject = new OrdersRootObject()
            {
                Orders = ordersForCustomer
            };

            return Ok(ordersRootObject);
        }
    }
}