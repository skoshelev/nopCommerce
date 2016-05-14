using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.Orders;
using Nop.Plugin.Api.Models.OrdersParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Orders
{
    [TestFixture]
    public class OrdersControllerTests_GetOrders
    {
        [Test]
        public void WhenSomeValidParametersPassed_ShouldCallTheServiceWithTheSameParameters()
        {
            var parameters = new OrdersParametersModel()
            {
                SinceId = Configurations.DefaultSinceId + 1, // some different than default since id
                CreatedAtMin = DateTime.Now,
                CreatedAtMax = DateTime.Now,
                Page = Configurations.DefaultPageValue + 1, // some different than default page
                Limit = Configurations.MinLimit + 1, // some different than default limit
                Ids = new List<int>() {1, 2, 3}
            };

            //Arange
            IOrderApiService orderApiServiceMock = MockRepository.GenerateMock<IOrderApiService>();
            orderApiServiceMock.Expect(x => x.GetOrders(parameters.Ids,
                                                    parameters.CreatedAtMin,
                                                    parameters.CreatedAtMax,
                                                    parameters.Limit,
                                                    parameters.Page,
                                                    parameters.SinceId)).Return(new List<Order>());

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new OrdersController(orderApiServiceMock, jsonFieldsSerializer);

            //Act
            cut.GetOrders(parameters);

            //Assert
            orderApiServiceMock.VerifyAllExpectations();
        }

        [Test]
        public void WhenSomeOrdersExist_ShouldCallTheSerializerWithTheseOrders()
        {
            var returnedOrdersCollection = new List<Order>()
            {
                new Order(),
                new Order()
            };

            var parameters = new OrdersParametersModel();

            //Arange
            IOrderApiService orderApiServiceStub = MockRepository.GenerateStub<IOrderApiService>();
            orderApiServiceStub.Stub(x => x.GetOrders()).IgnoreArguments().Return(returnedOrdersCollection);

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();

            var cut = new OrdersController(orderApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.GetOrders(parameters);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(
                x => x.Serialize(Arg<OrdersRootObject>.Matches(r => r.Orders.Count == returnedOrdersCollection.Count),
                Arg<string>.Is.Equal(parameters.Fields)));
        }

        [Test]
        public void WhenNoOrdersExist_ShouldCallTheSerializerWithNoOrders()
        {
            var returnedOrdersDtoCollection = new List<Order>();

            var parameters = new OrdersParametersModel();

            //Arange
            IOrderApiService orderApiServiceStub = MockRepository.GenerateStub<IOrderApiService>();
            orderApiServiceStub.Stub(x => x.GetOrders()).IgnoreArguments().Return(returnedOrdersDtoCollection);

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();
            
            var cut = new OrdersController(orderApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.GetOrders(parameters);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(
                x => x.Serialize(Arg<OrdersRootObject>.Matches(r => r.Orders.Count == returnedOrdersDtoCollection.Count),
                Arg<string>.Is.Equal(parameters.Fields)));
        }

        [Test]
        public void WhenFieldsParametersPassed_ShouldCallTheSerializerWithTheSameFields()
        {
            var parameters = new OrdersParametersModel()
            {
                Fields = "id,paymentstatus"
            };

            var returnedOrdersDtoCollection = new List<Order>();

            //Arange
            IOrderApiService orderApiServiceStub = MockRepository.GenerateStub<IOrderApiService>();
            orderApiServiceStub.Stub(x => x.GetOrders()).IgnoreArguments().Return(returnedOrdersDtoCollection);

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();

            var cut = new OrdersController(orderApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.GetOrders(parameters);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(
                x => x.Serialize(Arg<OrdersRootObject>.Is.Anything, Arg<string>.Is.Equal(parameters.Fields)));
        }

        [Test]
        [TestCase(Configurations.MinLimit - 1)]
        [TestCase(Configurations.MaxLimit + 1)]
        public void WhenInvalidLimitParameterPassed_ShouldReturnBadRequest(int invalidLimit)
        {
            var parameters = new OrdersParametersModel()
            {
                Limit = invalidLimit
            };

            //Arange
            IOrderApiService orderApiServiceStub = MockRepository.GenerateStub<IOrderApiService>();

            IJsonFieldsSerializer jsonFieldsSerializerStub = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new OrdersController(orderApiServiceStub, jsonFieldsSerializerStub);

            //Act
            IHttpActionResult result = cut.GetOrders(parameters);

            //Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public void WhenInvalidPageParameterPassed_ShouldReturnBadRequest(int invalidPage)
        {
            var parameters = new OrdersParametersModel()
            {
                Page = invalidPage
            };

            //Arange
            IOrderApiService orderApiServiceStub = MockRepository.GenerateStub<IOrderApiService>();

            IJsonFieldsSerializer jsonFieldsSerializerStub = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new OrdersController(orderApiServiceStub, jsonFieldsSerializerStub);

            //Act
            IHttpActionResult result = cut.GetOrders(parameters);

            //Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
        }
    }
}