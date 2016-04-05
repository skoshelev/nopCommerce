using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.DTOs.Orders;
using Nop.Plugin.Api.Models.OrdersParameters;
using Nop.Plugin.Api.MVC;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Orders
{
    [TestFixture]
    public class OrdersControllerTests_GetOrdersCount
    {
        [Test]
        public void WhenNoOrdersExist_ShouldReturnOKResultWithCountEqualToZero()
        {
            // arrange
            var ordersCountParameters = new OrdersCountParametersModel();

            var ordersApiServiceStub = MockRepository.GenerateStub<IOrderApiService>();
            ordersApiServiceStub.Stub(x => x.GetOrdersCount()).IgnoreArguments().Return(0);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();
            jsonFieldsSerializer.Stub(x => x.Serialize(null, null)).Return(string.Empty);

            var cut = new OrdersController(ordersApiServiceStub, jsonFieldsSerializer);

            // act
            IHttpActionResult result = cut.GetOrdersCount(ordersCountParameters);

            // assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<OrdersCountRootObject>>(result);
            Assert.AreEqual(0, ((OkNegotiatedContentResult<OrdersCountRootObject>)result).Content.Count);
        }

        [Test]
        public void WhenSingleOrderExists_ShouldReturnOKWithCountEqualToOne()
        {
            // arrange
            var ordersCountParameters = new OrdersCountParametersModel();

            var ordersApiServiceStub = MockRepository.GenerateStub<IOrderApiService>();
            ordersApiServiceStub.Stub(x => x.GetOrdersCount()).IgnoreArguments().Return(1);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();
            jsonFieldsSerializer.Stub(x => x.Serialize(null, null)).Return(string.Empty);

            var cut = new OrdersController(ordersApiServiceStub, jsonFieldsSerializer);

            // act
            IHttpActionResult result = cut.GetOrdersCount(ordersCountParameters);

            // assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<OrdersCountRootObject>>(result);
            Assert.AreEqual(1, ((OkNegotiatedContentResult<OrdersCountRootObject>)result).Content.Count);
        }

        [Test]
        public void WhenCertainNumberOfOrdersExist_ShouldReturnOKWithCountEqualToSameNumberOfOrders()
        {
            // arrange
            var ordersCountParameters = new OrdersCountParametersModel();

            var customersApiServiceStub = MockRepository.GenerateStub<IOrderApiService>();
            customersApiServiceStub.Stub(x => x.GetOrdersCount()).IgnoreArguments().Return(20000);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();
            jsonFieldsSerializer.Stub(x => x.Serialize(null, null)).Return(string.Empty);

            var cut = new OrdersController(customersApiServiceStub, jsonFieldsSerializer);

            // act
            IHttpActionResult result = cut.GetOrdersCount(ordersCountParameters);

            // assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<OrdersCountRootObject>>(result);
            Assert.AreEqual(20000, ((OkNegotiatedContentResult<OrdersCountRootObject>)result).Content.Count);
        }

        [Test]
        public void WhenSomeValidParametersPassed_ShouldCallTheServiceWithTheSameParameters()
        {
            var parameters = new OrdersCountParametersModel()
            {
                CreatedAtMin = DateTime.Now,
                CreatedAtMax = DateTime.Now,
                Status = OrderStatus.Complete,
                ShippingStatus = ShippingStatus.Delivered,
                PaymentStatus = PaymentStatus.Authorized,
                CustomerId = 10
            };

            //Arange
            IOrderApiService orderApiServiceMock = MockRepository.GenerateMock<IOrderApiService>();
            orderApiServiceMock.Expect(x => x.GetOrdersCount(parameters.CreatedAtMin,
                                                            parameters.CreatedAtMax,
                                                            parameters.Status,
                                                            parameters.PaymentStatus,
                                                            parameters.ShippingStatus,
                                                            parameters.CustomerId)).Return(1);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new OrdersController(orderApiServiceMock, jsonFieldsSerializer);

            //Act
            cut.GetOrdersCount(parameters);

            //Assert
            orderApiServiceMock.VerifyAllExpectations();
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public void WhenInvalidCustomerIdParameterPassed_ShouldCallTheServiceWithThisCustomerId(int invalidCustomerId)
        {
            var parameters = new OrdersCountParametersModel()
            {
                CustomerId = invalidCustomerId
            };

            //Arange
            IOrderApiService orderApiServiceMock = MockRepository.GenerateMock<IOrderApiService>();

            orderApiServiceMock.Expect(x => x.GetOrdersCount(parameters.CreatedAtMin,
                                                            parameters.CreatedAtMax,
                                                            parameters.Status,
                                                            parameters.PaymentStatus,
                                                            parameters.ShippingStatus,
                                                            parameters.CustomerId)).Return(0);

            IJsonFieldsSerializer jsonFieldsSerializerStub = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new OrdersController(orderApiServiceMock, jsonFieldsSerializerStub);

            //Act
            cut.GetOrdersCount(parameters);

            //Assert
            orderApiServiceMock.VerifyAllExpectations();
        }
    }
}