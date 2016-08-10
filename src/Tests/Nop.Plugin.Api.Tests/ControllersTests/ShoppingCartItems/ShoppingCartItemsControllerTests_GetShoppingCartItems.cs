using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;
using AutoMock;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.ShoppingCarts;
using Nop.Plugin.Api.Maps;
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
            var autoMocker = new RhinoAutoMocker<ShoppingCartItemsController>();
           
            //Act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetShoppingCartItems(parameters);

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
            var autoMocker = new RhinoAutoMocker<ShoppingCartItemsController>();

            //Act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetShoppingCartItems(parameters);

            //Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
        }

        [Test]
        public void WhenSomeValidParametersPassed_ShouldCallTheServiceWithTheSameParameters()
        {
            var parameters = new ShoppingCartItemsParametersModel();

            //Arange
            var autoMocker = new RhinoAutoMocker<ShoppingCartItemsController>();

            autoMocker.Get<IShoppingCartItemApiService>().Expect(x => x.GetShoppingCartItems(0, parameters.CreatedAtMin,
                                                                               parameters.CreatedAtMax, parameters.UpdatedAtMin,
                                                                               parameters.UpdatedAtMax, parameters.Limit,
                                                                               parameters.Page)).Return(new List<ShoppingCartItem>());

            //Act
            autoMocker.ClassUnderTest.GetShoppingCartItems(parameters);

            //Assert
            autoMocker.Get<IShoppingCartItemApiService>().VerifyAllExpectations();
        }

        [Test]
        public void WhenNoShoppingCartItemsExist_ShouldCallTheSerializerWithNoShoppingCartItems()
        {
            var returnedShoppingCartItemsCollection = new List<ShoppingCartItem>();

            var parameters = new ShoppingCartItemsParametersModel();

            //Arange
            var autoMocker = new RhinoAutoMocker<ShoppingCartItemsController>();

            autoMocker.Get<IShoppingCartItemApiService>().Stub(x => x.GetShoppingCartItems()).Return(returnedShoppingCartItemsCollection).IgnoreArguments();

            //Act
            autoMocker.ClassUnderTest.GetShoppingCartItems(parameters);

            //Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
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
            var autoMocker = new RhinoAutoMocker<ShoppingCartItemsController>();
            autoMocker.Get<IShoppingCartItemApiService>().Stub(x => x.GetShoppingCartItems()).Return(new List<ShoppingCartItem>()).IgnoreArguments();

            //Act
            autoMocker.ClassUnderTest.GetShoppingCartItems(parameters);

            //Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(Arg<ShoppingCartItemsRootObject>.Is.Anything, Arg<string>.Is.Equal(parameters.Fields)));
        }

        [Test]
        public void WhenSomeProductsExist_ShouldCallTheSerializerWithTheseProducts()
        {
            MappingExtensions.Maps.CreateMap<ShoppingCartItem, ShoppingCartItemDto>();

            var returnedShoppingCartItemsDtoCollection = new List<ShoppingCartItem>()
            {
                new ShoppingCartItem(),
                new ShoppingCartItem()
            };

            var parameters = new ShoppingCartItemsParametersModel();

            //Arange
            var autoMocker = new RhinoAutoMocker<ShoppingCartItemsController>();
            
            autoMocker.Get<IShoppingCartItemApiService>().Stub(x => x.GetShoppingCartItems()).Return(returnedShoppingCartItemsDtoCollection).IgnoreArguments();
            //Act
            autoMocker.ClassUnderTest.GetShoppingCartItems(parameters);

            //Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(Arg<ShoppingCartItemsRootObject>.Matches(r => r.ShoppingCartItems.Count == returnedShoppingCartItemsDtoCollection.Count),
                Arg<string>.Is.Equal(parameters.Fields)));
        }
    }
}