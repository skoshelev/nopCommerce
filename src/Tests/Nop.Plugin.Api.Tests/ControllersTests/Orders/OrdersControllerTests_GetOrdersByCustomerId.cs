using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Orders
{
    [TestFixture]
    public class OrdersControllerTests_GetOrdersByCustomerId
    {
        [Test]
        [TestCase(-5)]
        [TestCase(0)]
        [TestCase(10)]
        [TestCase(int.MaxValue)]
        public void WhenCustomerIdIsPassed_ShouldCallTheServiceWithThePassedParameters(int customerId)
        {
            // Arange
            IOrderApiService orderApiServiceMock = MockRepository.GenerateMock<IOrderApiService>();
            orderApiServiceMock.Expect(x => x.GetOrdersByCustomerId(customerId)).Return(new List<Order>());

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new OrdersController(orderApiServiceMock, jsonFieldsSerializer);

            // Act
            cut.GetOrdersByCustomerId(customerId);

            // Assert
            orderApiServiceMock.VerifyAllExpectations();
        }
    }
}