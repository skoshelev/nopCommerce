using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.ShoppingCarts;
using Nop.Plugin.Api.MappingExtensions;
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

            IShoppingCartItemApiService shoppingCartItemsApiServiceStub = MockRepository.GenerateStub<IShoppingCartItemApiService>();
            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new ShoppingCartItemsController(shoppingCartItemsApiServiceStub, jsonFieldsSerializer);

            // Act
            IHttpActionResult result = cut.GetShoppingCartItemsByCustomerId(nonPositiveCustomerId, parameters);

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

            IShoppingCartItemApiService shoppingCartItemsApiServiceStub = MockRepository.GenerateStub<IShoppingCartItemApiService>();

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();
            jsonFieldsSerializer.Stub(x => x.Serialize(null, null)).Return(string.Empty);

            var cut = new ShoppingCartItemsController(shoppingCartItemsApiServiceStub, jsonFieldsSerializer);

            // Act
            cut.GetShoppingCartItemsByCustomerId(negativeShoppingCartItemsId, parameters);

            // Assert
            shoppingCartItemsApiServiceStub.AssertWasNotCalled(x => x.GetShoppingCartItems(negativeShoppingCartItemsId));
        }

        [Test]
        public void WhenIdIsPositiveNumberButNoSuchShoppingCartItemsExists_ShouldReturn404NotFound()
        {
            int nonExistingShoppingCartItemId = 5;
            var parameters = new ShoppingCartItemsForCustomerParametersModel();

            // Arange
            IShoppingCartItemApiService shoppingCartItemsApiServiceStub = MockRepository.GenerateStub<IShoppingCartItemApiService>();
            shoppingCartItemsApiServiceStub.Stub(x => x.GetShoppingCartItems(nonExistingShoppingCartItemId)).Return(null);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new ShoppingCartItemsController(shoppingCartItemsApiServiceStub, jsonFieldsSerializer);

            // Act
            IHttpActionResult result = cut.GetShoppingCartItemsByCustomerId(nonExistingShoppingCartItemId, parameters);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void WhenIdEqualsToExistingShoppingCartItemId_ShouldSerializeThatShoppingCartItem()
        {
            Maps.CreateMap<ShoppingCartItem, ShoppingCartItemDto>();

            int existingShoppingCartItemId = 5;
            var existingShoppingCartItems = new List<ShoppingCartItem>()
            {
                new ShoppingCartItem() {Id = existingShoppingCartItemId}
            };

            var parameters = new ShoppingCartItemsForCustomerParametersModel();

            // Arange
            IShoppingCartItemApiService shoppingCartItemsApiServiceStub = MockRepository.GenerateStub<IShoppingCartItemApiService>();
            shoppingCartItemsApiServiceStub.Stub(x => x.GetShoppingCartItems(existingShoppingCartItemId)).Return(existingShoppingCartItems);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateMock<IJsonFieldsSerializer>();

            var cut = new ShoppingCartItemsController(shoppingCartItemsApiServiceStub, jsonFieldsSerializer);

            // Act
            cut.GetShoppingCartItemsByCustomerId(existingShoppingCartItemId, parameters);

            // Assert
            jsonFieldsSerializer.AssertWasCalled(
                x => x.Serialize(
                    Arg<ShoppingCartItemsRootObject>.Matches(
                        objectToSerialize =>
                               objectToSerialize.ShoppingCartItems.Count == 1 &&
                               objectToSerialize.ShoppingCartItems[0].Id == existingShoppingCartItemId),
                    Arg<string>.Is.Equal("")));
        }

        [Test]
        public void WhenIdEqualsToExistingShoppingCartItemIdAndFieldsSet_ShouldReturnJsonForThatShoppingCartItemWithSpecifiedFields()
        {
            Maps.CreateMap<ShoppingCartItem, ShoppingCartItemDto>();

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
            IShoppingCartItemApiService shoppingCartItemsApiServiceStub = MockRepository.GenerateStub<IShoppingCartItemApiService>();
            shoppingCartItemsApiServiceStub.Stub(x => x.GetShoppingCartItems(existingShoppingCartItemId)).Return(existingShoppingCartItems);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateMock<IJsonFieldsSerializer>();

            var cut = new ShoppingCartItemsController(shoppingCartItemsApiServiceStub, jsonFieldsSerializer);

            // Act
            cut.GetShoppingCartItemsByCustomerId(existingShoppingCartItemId, parameters);

            // Assert
            jsonFieldsSerializer.AssertWasCalled(
                x => x.Serialize(
                    Arg<ShoppingCartItemsRootObject>.Matches(objectToSerialize => objectToSerialize.ShoppingCartItems[0].Id == existingShoppingCartItemId),
                    Arg<string>.Matches(fieldsParameter => fieldsParameter == parameters.Fields)));
        }
    }
}