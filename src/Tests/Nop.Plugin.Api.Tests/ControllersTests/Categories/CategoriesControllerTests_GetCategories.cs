using System;
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
    [TestFixture]
    public class CategoriesControllerTests_GetCategories
    {
        // TODO: Move these tests when the specific module is ready
        //[TestCase("bbb")]
        //[TestCase(",,,,")]
        //[TestCase("asd,asda")]
        //[TestCase("1234323232323223")]
        // The cases above should be used for the tests of the module that converts the ids from string to list<int>
        // WhenNonNumericIdsParameterPassed_ShouldCallTheServiceWithNullIds(string ids)

        // TODO: Move these tests when the specific module is ready
        //[TestCase("1")]
        //[TestCase("1,1")]
        //[TestCase("1,sasa")]
        //[TestCase("asda,1,sasa,aa")]
        // The cases above should be used for the tests of the module that converts the ids from string to list<int>
        // WhenSigleValidNumericIdParameterPassed_ShouldCallTheServiceWithThatId(List<int> ids)

        // TODO: Move these tests when the specific module is ready
        //[TestCase("1,2")]
        //[TestCase("1, 2,1, 2 ")]
        //[TestCase("1,sasa, 2")]
        //[TestCase("asda,1,sasa, 2, aa,2 , 1")]
        // The cases above should be used for the tests of the module that converts the ids from string to list<int>
        // WhenSeveralNumericIdsParameterPassed_ShouldCallTheServiceWithTheseIds(string ids)

        // TODO: Move these tests when the specific module is ready
        //[TestCase("somestatus")]
        //[TestCase("Published")]
        //[TestCase("Unpublished")]
        // The cases above should be used for the tests of the module that converts the status from string to bool?
        // WhenInvalidStatusParameterPassed_ShouldCallTheServiceWithAnyStatus(string status)

        // TODO: Move these tests when the specific module is ready
        //[TestCase("published")]
        //[TestCase("unpublished")]
        //[TestCase("any")]
        // The cases above should be used for the tests of the module that converts the status from string to bool?
        // WhenValidStatusParameterPassed_ShouldCallTheServiceWithSameStatus(string validStatus)

        [Test]
        [TestCase(Configurations.MinLimit - 1)]
        [TestCase(Configurations.MaxLimit + 1)]
        public void WhenInvalidLimitParameterPassed_ShouldReturnBadRequest(int invalidLimit)
        {
            var parameters = new CategoriesParametersModel()
            {
                Limit = invalidLimit
            };

            //Arange
            ICategoryApiService categoryApiServiceStub = MockRepository.GenerateStub<ICategoryApiService>();

            IJsonFieldsSerializer jsonFieldsSerializerStub = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            CategoriesController cut = new CategoriesController(categoryApiServiceStub, jsonFieldsSerializerStub);

            //Act
            IHttpActionResult result = cut.GetCategories(parameters);

            //Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public void WhenInvalidPageParameterPassed_ShouldReturnBadRequest(int invalidPage)
        {
            var parameters = new CategoriesParametersModel()
            {
                Page = invalidPage
            };

            //Arange
            ICategoryApiService categoryApiServiceStub = MockRepository.GenerateStub<ICategoryApiService>();

            IJsonFieldsSerializer jsonFieldsSerializerStub = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new CategoriesController(categoryApiServiceStub, jsonFieldsSerializerStub);

            //Act
            IHttpActionResult result = cut.GetCategories(parameters);

            //Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
        }

        [Test]
        public void WhenSomeValidParametersPassed_ShouldCallTheServiceWithTheSameParameters()
        {
            var parameters = new CategoriesParametersModel();

            //Arange
            ICategoryApiService categoryApiServiceMock = MockRepository.GenerateMock<ICategoryApiService>();

            categoryApiServiceMock.Expect(x => x.GetCategories(parameters.Ids,
                                                    parameters.CreatedAtMin,
                                                    parameters.CreatedAtMax,
                                                    parameters.UpdatedAtMin,
                                                    parameters.UpdatedAtMax,
                                                    parameters.Limit,
                                                    parameters.Page,
                                                    parameters.SinceId,
                                                    parameters.ProductId,
                                                    parameters.PublishedStatus)).Return(new List<Category>());

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new CategoriesController(categoryApiServiceMock, jsonFieldsSerializer);

            //Act
            cut.GetCategories(parameters);

            //Assert
            categoryApiServiceMock.VerifyAllExpectations();
        }

        [Test]
        public void WhenSomeCategoriesExist_ShouldCallTheSerializerWithTheseCategories()
        {
            Maps.CreateMap<Category, CategoryDto>();

            var returnedCategoriesCollection = new List<Category>()
            {
                new Category(),
                new Category()
            };

            var parameters = new CategoriesParametersModel();

            //Arange
            ICategoryApiService categoryApiServiceStub = MockRepository.GenerateStub<ICategoryApiService>();
            categoryApiServiceStub.Stub(x => x.GetCategories()).Return(returnedCategoriesCollection);

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();

            var cut = new CategoriesController(categoryApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.GetCategories(parameters);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(
                x => x.Serialize(Arg<CategoriesRootObject>.Matches(r => r.Categories.Count == 2),
                Arg<string>.Is.Equal(parameters.Fields)));
        }

        [Test]
        public void WhenAnyFieldsParametersPassed_ShouldCallTheSerializerWithTheSameFields()
        {
            var parameters = new CategoriesParametersModel()
            {
                Fields = "id,name"
            };

            //Arange
            ICategoryApiService categoryApiServiceStub = MockRepository.GenerateStub<ICategoryApiService>();
            categoryApiServiceStub.Stub(x => x.GetCategories()).Return(new List<Category>());

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();

            var cut = new CategoriesController(categoryApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.GetCategories(parameters);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(
                x => x.Serialize(Arg<CategoriesRootObject>.Is.Anything, Arg<string>.Is.Equal(parameters.Fields)));
        }

        [Test]
        public void WhenNoCategoriesExist_ShouldCallTheSerializerWithRootObjectWithoutCategories()
        {
            var parameters = new CategoriesParametersModel();

            //Arange
            ICategoryApiService categoryApiServiceStub = MockRepository.GenerateStub<ICategoryApiService>();
            categoryApiServiceStub.Stub(x => x.GetCategories()).Return(new List<Category>());

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();


            var cut = new CategoriesController(categoryApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.GetCategories(parameters);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(
                x => x.Serialize(Arg<CategoriesRootObject>.Matches(r => r.Categories.Count == 0),
                Arg<string>.Is.Equal(parameters.Fields)));
        }
        
        
    }
}