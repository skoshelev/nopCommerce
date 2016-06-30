using System.Web.Http;
using System.Web.Http.Results;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.Orders;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Orders
{
    [TestFixture]
    public class OrdersControllerTests_GetOrderById
    {
        [Test]
        public void WhenIdIsPositiveNumberButNoSuchOrderWithSuchIdExists_ShouldReturn404NotFound()
        {
            int nonExistingOrderId = 5;

            // Arange
            IOrderApiService orderApiServiceStub = MockRepository.GenerateStub<IOrderApiService>();
            orderApiServiceStub.Stub(x => x.GetOrderById(nonExistingOrderId)).Return(null);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new OrdersController(orderApiServiceStub, jsonFieldsSerializer);

            // Act
            IHttpActionResult result = cut.GetOrderById(nonExistingOrderId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldReturn404NotFound(int nonPositiveOrderId)
        {
            // Arange
            IOrderApiService orderApiServiceStub = MockRepository.GenerateStub<IOrderApiService>();
            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new OrdersController(orderApiServiceStub, jsonFieldsSerializer);

            // Act
            IHttpActionResult result = cut.GetOrderById(nonPositiveOrderId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldNotCallOrderApiService(int negativeOrderId)
        {
            // Arange
            IOrderApiService orderApiServiceMock = MockRepository.GenerateMock<IOrderApiService>();
            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new OrdersController(orderApiServiceMock, jsonFieldsSerializer);

            // Act
            cut.GetOrderById(negativeOrderId);

            // Assert
            orderApiServiceMock.AssertWasNotCalled(x => x.GetOrderById(negativeOrderId));
        }

        [Test]
        public void WhenIdEqualsToExistingOrderId_ShouldSerializeThatOrder()
        {
            int existingOrderId = 5;
            var existingOrderDto = new Order() { Id = existingOrderId };

            // Arange
            IOrderApiService orderApiServiceStub = MockRepository.GenerateStub<IOrderApiService>();
            orderApiServiceStub.Stub(x => x.GetOrderById(existingOrderId)).Return(existingOrderDto);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateMock<IJsonFieldsSerializer>();

            var cut = new OrdersController(orderApiServiceStub, jsonFieldsSerializer);

            // Act
            cut.GetOrderById(existingOrderId);

            // Assert
            jsonFieldsSerializer.AssertWasCalled(
                x => x.Serialize(
                    Arg<OrdersRootObject>.Matches(objectToSerialize => objectToSerialize.Orders[0].Id == existingOrderDto.Id),
                Arg<string>.Matches(fields => fields == "")));
        }
    }
}