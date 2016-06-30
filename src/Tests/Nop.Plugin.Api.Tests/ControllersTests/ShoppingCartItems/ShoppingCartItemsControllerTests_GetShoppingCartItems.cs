using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Constants;
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
    public class ShoppingCartItemsControllerTests_GetShoppingCartItems
    {
        [Test]
        [TestCase(Configurations.MinLimit - 1)]
        [TestCase(Configurations.MaxLimit + 1)]
        public void WhenInvalidLimitParameterPassed_ShouldReturnBadRequest(int invalidLimit)
        {
            var parameters = new ShoppingCartItemsParametersModel()
            {
                Limit = invalidLimit
            };

            //Arange
            IShoppingCartItemApiService shoppingCartItemsApiServiceStub = MockRepository.GenerateStub<IShoppingCartItemApiService>();

            IJsonFieldsSerializer jsonFieldsSerializerStub = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new ShoppingCartItemsController(shoppingCartItemsApiServiceStub, jsonFieldsSerializerStub);

            //Act
            IHttpActionResult result = cut.GetShoppingCartItems(parameters);

            //Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public void WhenInvalidPageParameterPassed_ShouldReturnBadRequest(int invalidPage)
        {
            var parameters = new ShoppingCartItemsParametersModel()
            {
                Page = invalidPage
            };

            //Arange
            IShoppingCartItemApiService shoppingCartItemsApiServiceStub = MockRepository.GenerateStub<IShoppingCartItemApiService>();

            IJsonFieldsSerializer jsonFieldsSerializerStub = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new ShoppingCartItemsController(shoppingCartItemsApiServiceStub, jsonFieldsSerializerStub);

            //Act
            IHttpActionResult result = cut.GetShoppingCartItems(parameters);

            //Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
        }

        [Test]
        public void WhenSomeValidParametersPassed_ShouldCallTheServiceWithTheSameParameters()
        {
            var parameters = new ShoppingCartItemsParametersModel();

            //Arange
            IShoppingCartItemApiService shoppingCartItemsApiServiceMock = MockRepository.GenerateMock<IShoppingCartItemApiService>();

            shoppingCartItemsApiServiceMock.Expect(x => x.GetShoppingCartItems(0, parameters.CreatedAtMin,
                                                                               parameters.CreatedAtMax, parameters.UpdatedAtMin,
                                                                               parameters.UpdatedAtMax, parameters.Limit, 
                                                                               parameters.Page)).Return(new List<ShoppingCartItem>());

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new ShoppingCartItemsController(shoppingCartItemsApiServiceMock, jsonFieldsSerializer);

            //Act
            cut.GetShoppingCartItems(parameters);

            //Assert
            shoppingCartItemsApiServiceMock.VerifyAllExpectations();
        }

        [Test]
        public void WhenNoShoppingCartItemsExist_ShouldCallTheSerializerWithNoShoppingCartItems()
        {
            var returnedShoppingCartItemsCollection = new List<ShoppingCartItem>();

            var parameters = new ShoppingCartItemsParametersModel();

            //Arange
            IShoppingCartItemApiService shoppingCartItemsApiServiceMock = MockRepository.GenerateStub<IShoppingCartItemApiService>();
            shoppingCartItemsApiServiceMock.Stub(x => x.GetShoppingCartItems()).Return(returnedShoppingCartItemsCollection).IgnoreArguments();

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();

            var cut = new ShoppingCartItemsController(shoppingCartItemsApiServiceMock, jsonFieldsSerializerMock);

            //Act
            cut.GetShoppingCartItems(parameters);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(
                x => x.Serialize(Arg<ShoppingCartItemsRootObject>.Matches(r => r.ShoppingCartItems.Count == returnedShoppingCartItemsCollection.Count),
                Arg<string>.Is.Equal(parameters.Fields)));
        }

        [Test]
        public void WhenFieldsParametersPassed_ShouldCallTheSerializerWithTheSameFields()
        {
            var parameters = new ShoppingCartItemsParametersModel()
            {
                Fields = "id,quantity"
            };

            //Arange
            IShoppingCartItemApiService shoppingCartItemsApiServiceStub = MockRepository.GenerateStub<IShoppingCartItemApiService>();
            shoppingCartItemsApiServiceStub.Stub(x => x.GetShoppingCartItems()).Return(new List<ShoppingCartItem>()).IgnoreArguments();

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();

            var cut = new ShoppingCartItemsController(shoppingCartItemsApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.GetShoppingCartItems(parameters);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(
                x => x.Serialize(Arg<ShoppingCartItemsRootObject>.Is.Anything, Arg<string>.Is.Equal(parameters.Fields)));
        }

        [Test]
        public void WhenSomeProductsExist_ShouldCallTheSerializerWithTheseProducts()
        {
            Maps.CreateMap<ShoppingCartItem, ShoppingCartItemDto>();

            var returnedShoppingCartItemsDtoCollection = new List<ShoppingCartItem>()
            {
                new ShoppingCartItem(),
                new ShoppingCartItem()
            };

            var parameters = new ShoppingCartItemsParametersModel();

            //Arange
            IShoppingCartItemApiService shoppingCartItemsApiServiceStub = MockRepository.GenerateStub<IShoppingCartItemApiService>();
            shoppingCartItemsApiServiceStub.Stub(x => x.GetShoppingCartItems()).Return(returnedShoppingCartItemsDtoCollection).IgnoreArguments();

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();

            var cut = new ShoppingCartItemsController(shoppingCartItemsApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.GetShoppingCartItems(parameters);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(
                x => x.Serialize(Arg<ShoppingCartItemsRootObject>.Matches(r => r.ShoppingCartItems.Count == returnedShoppingCartItemsDtoCollection.Count),
                Arg<string>.Is.Equal(parameters.Fields)));
        }
    }
}