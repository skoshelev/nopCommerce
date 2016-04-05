using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.ProductCategoryMappings;
using Nop.Plugin.Api.DTOs.Products;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Models.ProductCategoryMappingsParameters;
using Nop.Plugin.Api.Models.ProductsParameters;
using Nop.Plugin.Api.MVC;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.ProductCategoryMappings
{
    [TestFixture]
    public class ProductCategoryMappingsControllerTests_GetMappings
    {
        [Test]
        public void WhenSomeValidParametersPassed_ShouldCallTheServiceWithTheSameParameters()
        {
            var parameters = new ProductCategoryMappingsParametersModel()
            {
                SinceId = Configurations.DefaultSinceId + 1, // some different than default since id
                Page = Configurations.DefaultPageValue + 1, // some different than default page
                Limit = Configurations.MinLimit + 1 // some different than default limit
            };

            //Arange
            IProductCategoryMappingsApiService productCategoryMappingApiServiceMock = MockRepository.GenerateMock<IProductCategoryMappingsApiService>();
            productCategoryMappingApiServiceMock.Expect(x => x.GetMappings(parameters.ProductId,
                                                                           parameters.CategoryId,
                                                                           parameters.Limit,
                                                                           parameters.Page,
                                                                           parameters.SinceId)).Return(new List<ProductCategory>());

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new ProductCategoryMappingsController(productCategoryMappingApiServiceMock, jsonFieldsSerializer);

            //Act
            cut.GetMappings(parameters);

            //Assert
            productCategoryMappingApiServiceMock.VerifyAllExpectations();
        }

        [Test]
        [TestCase(Configurations.MinLimit - 1)]
        [TestCase(Configurations.MaxLimit + 1)]
        public void WhenInvalidLimitParameterPassed_ShouldReturnBadRequest(int invalidLimit)
        {
            var parameters = new ProductCategoryMappingsParametersModel()
            {
                Limit = invalidLimit
            };

            //Arange
            IProductCategoryMappingsApiService productCategoryMappingApiServiceStub = MockRepository.GenerateStub<IProductCategoryMappingsApiService>();

            IJsonFieldsSerializer jsonFieldsSerializerStub = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new ProductCategoryMappingsController(productCategoryMappingApiServiceStub, jsonFieldsSerializerStub);

            //Act
            IHttpActionResult result = cut.GetMappings(parameters);

            //Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public void WhenInvalidPageParameterPassed_ShouldReturnBadRequest(int invalidPage)
        {
            var parameters = new ProductCategoryMappingsParametersModel()
            {
                Page = invalidPage
            };

            //Arange
            IProductCategoryMappingsApiService productCategoryMappingApiServiceStub = MockRepository.GenerateStub<IProductCategoryMappingsApiService>();

            IJsonFieldsSerializer jsonFieldsSerializerStub = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new ProductCategoryMappingsController(productCategoryMappingApiServiceStub, jsonFieldsSerializerStub);

            //Act
            IHttpActionResult result = cut.GetMappings(parameters);

            //Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
        }

        [Test]
        public void WhenNoProductCategoryMappingsExist_ShouldCallTheSerializerWithNoProductCategoryMappings()
        {
            var returnedMappingsCollection = new List<ProductCategory>();

            var parameters = new ProductCategoryMappingsParametersModel();

            //Arange
            IProductCategoryMappingsApiService productCategoryMappingApiServiceStub = MockRepository.GenerateStub<IProductCategoryMappingsApiService>();
            productCategoryMappingApiServiceStub.Stub(x => x.GetMappings()).IgnoreArguments().Return(returnedMappingsCollection);

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();

            var cut = new ProductCategoryMappingsController(productCategoryMappingApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.GetMappings(parameters);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(
                x => x.Serialize(Arg<ProductCategoryMappingsRootObject>.Matches(r => r.ProductCategoryMappingDtos.Count == returnedMappingsCollection.Count),
                Arg<string>.Is.Equal(parameters.Fields)));
        }

        [Test]
        public void WhenFieldsParametersPassed_ShouldCallTheSerializerWithTheSameFields()
        {
            var parameters = new ProductCategoryMappingsParametersModel()
            {
                Fields = "id,product_id"
            };

            //Arange
            IProductCategoryMappingsApiService productCategoryMappingApiServiceStub = MockRepository.GenerateStub<IProductCategoryMappingsApiService>();
            productCategoryMappingApiServiceStub.Stub(x => x.GetMappings()).Return(new List<ProductCategory>());

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();

            var cut = new ProductCategoryMappingsController(productCategoryMappingApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.GetMappings(parameters);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(
                x => x.Serialize(Arg<ProductCategoryMappingsRootObject>.Is.Anything, Arg<string>.Is.Equal(parameters.Fields)));
        }

        [Test]
        public void WhenSomeProductCategoryMappingsExist_ShouldCallTheSerializerWithTheseProductCategoryMappings()
        {
            Maps.CreateMap<ProductCategory, ProductCategoryMappingDto>();

            var returnedMappingsDtoCollection = new List<ProductCategory>()
            {
                new ProductCategory(),
                new ProductCategory()
            };

            var parameters = new ProductCategoryMappingsParametersModel();

            //Arange
            IProductCategoryMappingsApiService productCategoryMappingApiServiceStub = MockRepository.GenerateStub<IProductCategoryMappingsApiService>();
            productCategoryMappingApiServiceStub.Stub(x => x.GetMappings()).Return(returnedMappingsDtoCollection);

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();

            var cut = new ProductCategoryMappingsController(productCategoryMappingApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.GetMappings(parameters);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(
                x => x.Serialize(Arg<ProductCategoryMappingsRootObject>.Matches(r => r.ProductCategoryMappingDtos.Count == returnedMappingsDtoCollection.Count),
                Arg<string>.Is.Equal(parameters.Fields)));
        }
    }
}