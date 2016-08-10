using System.Web.Http;
using System.Web.Http.Results;
using AutoMock;
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
            var autoMocker = new RhinoAutoMocker<OrdersController>();

            autoMocker.Get<IOrderApiService>().Stub(x => x.GetOrderById(nonExistingOrderId)).Return(null);

            // Act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetOrderById(nonExistingOrderId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldReturn404NotFound(int nonPositiveOrderId)
        {
            // Arange
            var autoMocker = new RhinoAutoMocker<OrdersController>();

            // Act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetOrderById(nonPositiveOrderId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldNotCallOrderApiService(int negativeOrderId)
        {
            // Arange
            var autoMocker = new RhinoAutoMocker<OrdersController>();

            // Act
            autoMocker.ClassUnderTest.GetOrderById(negativeOrderId);

            // Assert
            autoMocker.Get<IOrderApiService>().AssertWasNotCalled(x => x.GetOrderById(negativeOrderId));
        }

        [Test]
        public void WhenIdEqualsToExistingOrderId_ShouldSerializeThatOrder()
        {
            int existingOrderId = 5;
            var existingOrderDto = new Order() { Id = existingOrderId };

            // Arange
            var autoMocker = new RhinoAutoMocker<OrdersController>();

            autoMocker.Get<IOrderApiService>().Stub(x => x.GetOrderById(existingOrderId)).Return(existingOrderDto);

            // Act
            autoMocker.ClassUnderTest.GetOrderById(existingOrderId);

            // Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(
                    Arg<OrdersRootObject>.Matches(objectToSerialize => objectToSerialize.Orders[0].Id == existingOrderDto.Id.ToString()),
                Arg<string>.Matches(fields => fields == "")));
        }
    }
}