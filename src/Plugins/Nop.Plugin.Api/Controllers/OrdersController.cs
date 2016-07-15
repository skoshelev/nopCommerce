using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using FluentValidation.Results;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTOs;
using Nop.Plugin.Api.DTOs.Orders;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Models.OrdersParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Plugin.Api.Validators;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class OrdersController : BaseApiController
    {
        private readonly IOrderApiService _orderApiService;
        private readonly IProductService _productService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly OrderSettings _orderSettings;
        private readonly IFactory<Order> _factory; 

        public OrdersController(IOrderApiService orderApiService, 
            IJsonFieldsSerializer jsonFieldsSerializer, 
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IProductService productService,
            IFactory<Order> factory, 
            OrderSettings orderSettings, 
            IOrderProcessingService orderProcessingService, 
            IOrderService orderService, 
            IShoppingCartService shoppingCartService)
            :base(jsonFieldsSerializer, aclService, customerService, storeMappingService, 
                 storeService, discountService, customerActivityService, localizationService)
        {
            _orderApiService = orderApiService;
            _factory = factory;
            _orderSettings = orderSettings;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
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
                return NotFound();
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

        [HttpPost]
        [ResponseType(typeof(OrdersRootObject))]
        public IHttpActionResult CreateOrder([ModelBinder(typeof(JsonModelBinder<OrderDto>))] Delta<OrderDto> orderDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
            {
                return Error();
            }
            
            // We doesn't have to check for value because this is done by the order validator.
            Customer customer = _customerService.GetCustomerById(orderDelta.Dto.CustomerId.Value);

            bool isVaild = ValidateCustomer(customer);

            if (!isVaild)
            {
                return Error();
            }

            bool shippingAddressRequired = false;

            bool shouldReturnError = AddOrderItemsToCart(orderDelta.Dto.OrderItems, customer, orderDelta.Dto.StoreId ?? 0,
                                        out shippingAddressRequired);

            if(shouldReturnError)
            {
                return Error();
            }

            if (shippingAddressRequired)
            {
                bool isValid = ValidateAddress(orderDelta.Dto.ShippingAddress, "shipping_address");

                if (!isValid)
                {
                    return Error();
                }
            }

            if (!_orderSettings.DisableBillingAddressCheckoutStep)
            {
                bool isValid = ValidateAddress(orderDelta.Dto.BillingAddress, "billing_address");

                if (!isValid)
                {
                    return Error();
                }
            }
            
            Order newOrder = _factory.Initialize();
            orderDelta.Merge(newOrder);

            customer.BillingAddress = newOrder.BillingAddress;
            customer.ShippingAddress = newOrder.ShippingAddress;

            newOrder.Customer = customer;

            var processPaymentRequest = new ProcessPaymentRequest();
            
            //prevent 2 orders being placed within an X seconds time frame
            if (!IsMinimumOrderPlacementIntervalValid(customer, newOrder.StoreId))
            {
                ModelState.AddModelError("order placement", _localizationService.GetResource("Checkout.MinOrderPlacementInterval"));

                return Error();
            }

            //place order
            processPaymentRequest.StoreId = newOrder.StoreId;
            processPaymentRequest.CustomerId = customer.Id;
            processPaymentRequest.PaymentMethodSystemName = newOrder.PaymentMethodSystemName;

            var placeOrderResult = _orderProcessingService.PlaceOrder(processPaymentRequest);

            if (!placeOrderResult.Success)
            {
                foreach (var error in placeOrderResult.Errors)
                {
                    ModelState.AddModelError("order placement", error);
                }

                return Error();
            } 

            _customerActivityService.InsertActivity("AddNewOrder",
                 _localizationService.GetResource("ActivityLog.AddNewOrder"), newOrder.Id);

            var ordersRootObject = new OrdersRootObject();

            OrderDto newOrderDto = placeOrderResult.PlacedOrder.ToDto();

            ordersRootObject.Orders.Add(newOrderDto);

            var json = _jsonFieldsSerializer.Serialize(ordersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        private bool AddOrderItemsToCart(ICollection<OrderItemDto> orderItems, Customer customer, int storeId, out bool shippingAddressRequired)
        {
            bool shouldReturnError = false;
            shippingAddressRequired = false;

            if (orderItems != null)
            {
                foreach (var orderItem in orderItems)
                {
                    var orderItemDtoValidator = new OrderItemDtoValidator();
                    ValidationResult validation = orderItemDtoValidator.Validate(orderItem);

                    if (validation.IsValid)
                    {
                        Product product = _productService.GetProductById(orderItem.ProductId.Value);

                        if (product != null)
                        {
                            IList<string> errors = _shoppingCartService.AddToCart(customer, product,
                                ShoppingCartType.ShoppingCart, storeId,
                                null, 0M, orderItem.RentalStartDateUtc, orderItem.RentalEndDateUtc,
                                orderItem.Quantity ?? 1);

                            if (errors.Count > 0)
                            {
                                foreach (var error in errors)
                                {
                                    ModelState.AddModelError("order", error);
                                }

                                shouldReturnError = true;
                            }

                            shippingAddressRequired |= product.IsShipEnabled;
                        }
                    }
                    else
                    {
                        foreach (var error in validation.Errors)
                        {
                            ModelState.AddModelError("order_item", error.ErrorMessage);
                        }

                        shouldReturnError = true;
                    }
                }
            }
            else
            {
                shouldReturnError = true;
            }

            return shouldReturnError;
        }
        
        private bool ValidateCustomer(Customer customer)
        {
            bool isValid = true;

            if (customer == null)
            {
                ModelState.AddModelError("customer", "Invalid customer");

                isValid = false;
            }

            return isValid;
        }

        private bool ValidateAddress(AddressDto address, string addressKind)
        {
            bool addressValid = true;

            if (address == null)
            {
                ModelState.AddModelError(addressKind, string.Format("{0} address required", addressKind));
                addressValid = false;
            }
            else 
            {
                var addressValidator = new AddressDtoValidator();
                ValidationResult validationResult = addressValidator.Validate(address);

                foreach (var validationFailure in validationResult.Errors)
                {
                    ModelState.AddModelError(addressKind, validationFailure.ErrorMessage);    
                }

                addressValid = validationResult.IsValid;
            }

            return addressValid;
        }

        private bool IsMinimumOrderPlacementIntervalValid(Customer customer, int storeId)
        {
            //prevent 2 orders being placed within an X seconds time frame
            if (_orderSettings.MinimumOrderPlacementInterval == 0)
                return true;

            var lastOrder = _orderService.SearchOrders(storeId: storeId,
                customerId: customer.Id, pageSize: 1)
                .FirstOrDefault();

            if (lastOrder == null)
                return true;

            var interval = DateTime.UtcNow - lastOrder.CreatedOnUtc;
            return interval.TotalSeconds > _orderSettings.MinimumOrderPlacementInterval;
        }
    }
}