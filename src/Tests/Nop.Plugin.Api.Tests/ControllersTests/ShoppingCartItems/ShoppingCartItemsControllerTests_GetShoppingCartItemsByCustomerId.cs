using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;
using AutoMock;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.ShoppingCarts;
using Nop.Plugin.Api.Models.ShoppingCartsParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.ShoppingCartItems
{
    [TestFixture]
    public class ShoppingCartItemsControllerTests_GetShoppingCartItemsByCustomerId
    {
        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldReturn404NotFound(int nonPositiveCustomerId)
        {
            // Arange
            var parameters = new ShoppingCartItemsForCustomerParametersModel();
            
            var autoMocker = new RhinoAutoMocker<ShoppingCartItemsController>();

            // Act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetShoppingCartItemsByCustomerId(nonPositiveCustomerId, parameters);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldNotCallShoppingCartItemsApiService(int negativeShoppingCartItemsId)
        {
            // Arange
            var parameters = new ShoppingCartItemsForCustomerParametersModel();
            
            var autoMocker = new RhinoAutoMocker<ShoppingCartItemsController>();

            autoMocker.Get<IJsonFieldsSerializer>().Stub(x => x.Serialize(null, null)).Return(string.Empty);

            // Act
            autoMocker.ClassUnderTest.GetShoppingCartItemsByCustomerId(negativeShoppingCartItemsId, parameters);

            // Assert
            autoMocker.Get<IShoppingCartItemApiService>().AssertWasNotCalled(x => x.GetShoppingCartItems(negativeShoppingCartItemsId));
        }

        [Test]
        public void WhenIdIsPositiveNumberButNoSuchShoppingCartItemsExists_ShouldReturn404NotFound()
        {
            int nonExistingShoppingCartItemId = 5;
            var parameters = new ShoppingCartItemsForCustomerParametersModel();

            // Arange
            var autoMocker = new RhinoAutoMocker<ShoppingCartItemsController>();

            autoMocker.Get<IShoppingCartItemApiService>().Stub(x => x.GetShoppingCartItems(nonExistingShoppingCartItemId)).Return(null);

            // Act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetShoppingCartItemsByCustomerId(nonExistingShoppingCartItemId, parameters);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void WhenIdEqualsToExistingShoppingCartItemId_ShouldSerializeThatShoppingCartItem()
        {
            MappingExtensions.Maps.CreateMap<ShoppingCartItem, ShoppingCartItemDto>();

            int existingShoppingCartItemId = 5;
            var existingShoppingCartItems = new List<ShoppingCartItem>()
            {
                new ShoppingCartItem() {Id = existingShoppingCartItemId}
            };

            var parameters = new ShoppingCartItemsForCustomerParametersModel();

            // Arange
            var autoMocker = new RhinoAutoMocker<ShoppingCartItemsController>();

            autoMocker.Get<IShoppingCartItemApiService>().Stub(x => x.GetShoppingCartItems(existingShoppingCartItemId)).Return(existingShoppingCartItems);

            // Act
            autoMocker.ClassUnderTest.GetShoppingCartItemsByCustomerId(existingShoppingCartItemId, parameters);

            // Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(
                    Arg<ShoppingCartItemsRootObject>.Matches(
                        objectToSerialize =>
                               objectToSerialize.ShoppingCartItems.Count == 1 &&
                               objectToSerialize.ShoppingCartItems[0].Id == existingShoppingCartItemId.ToString()),
                    Arg<string>.Is.Equal("")));
        }

        [Test]
        public void WhenIdEqualsToExistingShoppingCartItemIdAndFieldsSet_ShouldReturnJsonForThatShoppingCartItemWithSpecifiedFields()
        {
            MappingExtensions.Maps.CreateMap<ShoppingCartItem, ShoppingCartItemDto>();

            int existingShoppingCartItemId = 5;
            var existingShoppingCartItems = new List<ShoppingCartItem>()
            {
                new ShoppingCartItem() {Id = existingShoppingCartItemId}
            };
            
            var parameters = new ShoppingCartItemsForCustomerParametersModel()
            {
                Fields = "id,quantity"
            };

            // Arange
            var autoMocker = new RhinoAutoMocker<ShoppingCartItemsController>();

            autoMocker.Get<IShoppingCartItemApiService>().Stub(x => x.GetShoppingCartItems(existingShoppingCartItemId)).Return(existingShoppingCartItems);

            // Act
            autoMocker.ClassUnderTest.GetShoppingCartItemsByCustomerId(existingShoppingCartItemId, parameters);

            // Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(
                    Arg<ShoppingCartItemsRootObject>.Matches(objectToSerialize => objectToSerialize.ShoppingCartItems[0].Id == existingShoppingCartItemId.ToString()),
                    Arg<string>.Matches(fieldsParameter => fieldsParameter == parameters.Fields)));
        }
    }
}