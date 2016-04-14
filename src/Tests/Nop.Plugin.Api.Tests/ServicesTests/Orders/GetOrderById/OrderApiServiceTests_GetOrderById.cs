using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ServicesTests.Orders.GetOrderById
{
    [TestFixture]
    public class OrderApiServiceTests_GetOrderById
    {
        [Test]
        public void WhenNullIsReturnedByTheRepository_ShouldReturnNull()
        {
            int orderId = 3;
            
            // Arange
            var orderRepo = MockRepository.GenerateStub<IRepository<Order>>();
            orderRepo.Stub(x => x.GetById(orderId)).Return(null);
            
            // Act  
            var cut = new OrderApiService(orderRepo);
            var result = cut.GetOrderById(orderId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        [TestCase(-2)]
        [TestCase(0)]
        public void WhenNegativeOrZeroOrderIdPassed_ShouldReturnNull(int negativeOrZeroOrderId)
        {
            // Aranges
            var orderRepoMock = MockRepository.GenerateMock<IRepository<Order>>();

            // Act
            var cut = new OrderApiService(orderRepoMock);
            var result = cut.GetOrderById(negativeOrZeroOrderId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void WhenOrderIsReturnedByTheRepository_ShouldReturnTheSameOrder()
        {
            int orderId = 3;
            var order = new Order() { Id = 3 };

            // Arange
            var orderRepo = MockRepository.GenerateStub<IRepository<Order>>();
            orderRepo.Stub(x => x.GetById(orderId)).Return(order);
            
            // Act
            var cut = new OrderApiService(orderRepo);
            var result = cut.GetOrderById(orderId);

            // Assert
            Assert.AreSame(order, result);
        }
    }
}
