using System.Web.Http;
using System.Web.Http.Results;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Plugin.Api.Validators;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Categories
{
    [TestFixture]
    public class CategoriesControllerTests_GetCategoriesById
    {
        [Test]
        public void WhenIdIsPositiveNumberButNoSuchCategoryExists_ShouldReturn404NotFound()
        {
            int nonExistingCategoryId = 5;

            // Arange
            ICategoryApiService categoryApiServiceStub = MockRepository.GenerateStub<ICategoryApiService>();
            categoryApiServiceStub.Stub(x => x.GetCategoryById(nonExistingCategoryId)).Return(null);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            IParametersValidator parametersValidator = MockRepository.GenerateStub<IParametersValidator>();

            var cut = new CategoriesController(categoryApiServiceStub, jsonFieldsSerializer, parametersValidator);

            // Act
            IHttpActionResult result = cut.GetCategoryById(nonExistingCategoryId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldReturn404NotFound(int nonPositiveCategoryId)
        {
            // Arange
            ICategoryApiService categoryApiServiceStub = MockRepository.GenerateStub<ICategoryApiService>();
            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();
            IParametersValidator parametersValidator = MockRepository.GenerateStub<IParametersValidator>();

            var cut = new CategoriesController(categoryApiServiceStub, jsonFieldsSerializer, parametersValidator);

            // Act
            IHttpActionResult result = cut.GetCategoryById(nonPositiveCategoryId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldNotCallCategoryApiService(int negativeCategoryId)
        {
            // Arange
            ICategoryApiService categoryApiServiceMock = MockRepository.GenerateMock<ICategoryApiService>();

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();
            jsonFieldsSerializer.Stub(x => x.Serialize(null, null)).Return(string.Empty);

            IParametersValidator parametersValidator = MockRepository.GenerateStub<IParametersValidator>();

            var cut = new CategoriesController(categoryApiServiceMock, jsonFieldsSerializer, parametersValidator);

            // Act
            cut.GetCategoryById(negativeCategoryId);

            // Assert
            categoryApiServiceMock.AssertWasNotCalled(x => x.GetCategoryById(negativeCategoryId));
        }

        [Test]
        public void WhenIdEqualsToExistingCategoryId_ShouldSerializeThatCategory()
        {
            Maps.CreateMap<Category, CategoryDto>();

            int existingCategoryId = 5;
            var existingCategory = new Category() { Id = existingCategoryId };

            // Arange
            ICategoryApiService categoryApiServiceStub = MockRepository.GenerateStub<ICategoryApiService>();
            categoryApiServiceStub.Stub(x => x.GetCategoryById(existingCategoryId)).Return(existingCategory);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateMock<IJsonFieldsSerializer>();

            IParametersValidator parametersValidator = MockRepository.GenerateStub<IParametersValidator>();

            var cut = new CategoriesController(categoryApiServiceStub, jsonFieldsSerializer, parametersValidator);

            // Act
            cut.GetCategoryById(existingCategoryId);
            
            // Assert
            jsonFieldsSerializer.AssertWasCalled(
                x => x.Serialize(
                    Arg<CategoriesRootObject>.Matches(
                        objectToSerialize =>
                               objectToSerialize.Categories.Count == 1 &&
                               objectToSerialize.Categories[0].Id == existingCategory.Id.ToString() &&
                               objectToSerialize.Categories[0].Name == existingCategory.Name),
                    Arg<string>.Is.Equal("")));
        }

        [Test]
        public void WhenIdEqualsToExistingCategoryIdAndFieldsSet_ShouldReturnJsonForThatCategoryWithSpecifiedFields()
        {
            Maps.CreateMap<Category, CategoryDto>();

            int existingCategoryId = 5;
            var existingCategory = new Category() { Id = existingCategoryId, Name = "some category name" };
            string fields = "id,name";

            // Arange
            ICategoryApiService categoryApiServiceStub = MockRepository.GenerateStub<ICategoryApiService>();
            categoryApiServiceStub.Stub(x => x.GetCategoryById(existingCategoryId)).Return(existingCategory);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateMock<IJsonFieldsSerializer>();

            IParametersValidator parametersValidator = MockRepository.GenerateStub<IParametersValidator>();

            var cut = new CategoriesController(categoryApiServiceStub, jsonFieldsSerializer, parametersValidator);

            // Act
            cut.GetCategoryById(existingCategoryId, fields);

            // Assert
            jsonFieldsSerializer.AssertWasCalled(
                x => x.Serialize(
                    Arg<CategoriesRootObject>.Matches(objectToSerialize => objectToSerialize.Categories[0].Id == existingCategory.Id.ToString() &&
                                                                           objectToSerialize.Categories[0].Name == existingCategory.Name),
                Arg<string>.Matches(fieldsParameter => fieldsParameter == fields)));
        }
    }
}