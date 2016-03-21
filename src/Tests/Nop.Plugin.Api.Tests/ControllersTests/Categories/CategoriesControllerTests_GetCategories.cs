using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Models.CategoriesParameters;
using Nop.Plugin.Api.MVC;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Categories
{
    public class CategoriesControllerTests_GetCategories
    {
        [Test]
        public void WhenNoParametersPassed_ShouldCallTheServiceWithDefaultParameters()
        {
            var defaultParametersModel = new CategoriesParametersModel();

            //Arange
            ICategoryApiService categoryApiServiceMock = MockRepository.GenerateMock<ICategoryApiService>();
            categoryApiServiceMock.Stub(x => x.GetCategories()).Return(new List<Category>());

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new CategoriesController(categoryApiServiceMock, jsonFieldsSerializer);

            //Act
            cut.GetCategories(defaultParametersModel);

            //Assert
            categoryApiServiceMock.AssertWasCalled(x => x.GetCategories(null,
                                                    defaultParametersModel.CreatedAtMin,
                                                    defaultParametersModel.CreatedAtMax,
                                                    defaultParametersModel.UpdatedAtMin,
                                                    defaultParametersModel.UpdatedAtMax,
                                                    defaultParametersModel.Limit,
                                                    defaultParametersModel.Page,
                                                    defaultParametersModel.SinceId,
                                                    defaultParametersModel.ProductId,
                                                    defaultParametersModel.PublishedStatus));
        }

        [Test]
        public void WhenNoParametersPassedAndSomeCategoriesExist_ShouldCallTheSerializer()
        {
            Maps.CreateMap<Category, CategoryDto>();

            var expectedCategoriesCollection = new List<Category>()
            {
                new Category(),
                new Category()
            };

            var expectedRootObject = new CategoriesRootObject()
            {
                Categories = expectedCategoriesCollection.Select(x => x.ToDto()).ToList()
            };

            var defaultParameters = new CategoriesParametersModel();

            //Arange
            ICategoryApiService categoryApiServiceStub = MockRepository.GenerateStub<ICategoryApiService>();
            categoryApiServiceStub.Stub(x => x.GetCategories()).Return(expectedCategoriesCollection);

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();
            jsonFieldsSerializerMock.Expect(x => x.Serialize(expectedRootObject, defaultParameters.Fields));

            var cut = new CategoriesController(categoryApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.GetCategories(defaultParameters);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(x => x.Serialize(Arg<CategoriesRootObject>.Is.TypeOf,
                Arg<string>.Is.Equal(defaultParameters.Fields)));
        }

        [Test]
        public void WhenNoParametersPassedAndNoCategoriesExist_ShouldCallTheSerializer()
        {
            var expectedCategoriesCollection = new List<Category>();

            var expectedRootObject = new CategoriesRootObject()
            {
                Categories = expectedCategoriesCollection.Select(x => x.ToDto()).ToList()
            };

            var defaultParameters = new CategoriesParametersModel();

            //Arange
            ICategoryApiService categoryApiServiceStub = MockRepository.GenerateStub<ICategoryApiService>();
            categoryApiServiceStub.Stub(x => x.GetCategories()).Return(expectedCategoriesCollection);

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();
            jsonFieldsSerializerMock.Expect(x => x.Serialize(expectedRootObject, defaultParameters.Fields));

            var cut = new CategoriesController(categoryApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.GetCategories(defaultParameters);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(x => x.Serialize(Arg<CategoriesRootObject>.Is.TypeOf,
                Arg<string>.Is.Equal(defaultParameters.Fields)));
        }

        [Test]
        public void WhenFieldsParametersPassed_ShouldCallTheSerializerWithTheSameFields()
        {
            var defaultParametersModel = new CategoriesParametersModel()
            {
                Fields = "id,name"
            };

            //Arange
            ICategoryApiService categoryApiServiceStub = MockRepository.GenerateStub<ICategoryApiService>();
            categoryApiServiceStub.Stub(x => x.GetCategories()).Return(new List<Category>());

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();

            var cut = new CategoriesController(categoryApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.GetCategories(defaultParametersModel);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(
                x => x.Serialize(Arg<CategoriesRootObject>.Is.Anything, Arg<string>.Is.Equal(defaultParametersModel.Fields)));
        }

        [Test]
        [TestCase(Configurations.MinLimit)]
        [TestCase(Configurations.MinLimit - 1)]
        [TestCase(Configurations.MaxLimit + 1)]
        public void WhenInvalidLimitParameterPassed_ShouldReturnBadRequest(int invalidLimit)
        {
            var parametersModel = new CategoriesParametersModel()
            {
                Limit = invalidLimit
            };

            //Arange
            ICategoryApiService categoryApiServiceStub = MockRepository.GenerateStub<ICategoryApiService>();

            IJsonFieldsSerializer jsonFieldsSerializerStub = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            CategoriesController cut = new CategoriesController(categoryApiServiceStub, jsonFieldsSerializerStub);

            //Act
            IHttpActionResult result = cut.GetCategories(parametersModel);

            //Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public void WhenInvalidPageParameterPassed_ShouldReturnBadRequest(int invalidPage)
        {
            var parametersModel = new CategoriesParametersModel()
            {
                Limit = invalidPage
            };

            //Arange
            ICategoryApiService categoryApiServiceStub = MockRepository.GenerateStub<ICategoryApiService>();

            IJsonFieldsSerializer jsonFieldsSerializerStub = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new CategoriesController(categoryApiServiceStub, jsonFieldsSerializerStub);

            //Act
            IHttpActionResult result = cut.GetCategories(parametersModel);

            //Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
        }
    }
}