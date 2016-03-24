using System.Web.Http;
using System.Web.Http.Results;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.Categories;
using Nop.Plugin.Api.Models.CategoriesParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Plugin.Api.Validators;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Categories
{
    [TestFixture]
    public class CategoriesControllerTests_GetCategoriesCount
    {
        [Test]
        public void WhenNoCategoriesExist_ShouldReturnOKResultWithCountEqualToZero()
        {
            var categoriesCountParametersModel = new CategoriesCountParametersModel();

            // arrange
            var categoriesApiServiceStub = MockRepository.GenerateStub<ICategoryApiService>();
            categoriesApiServiceStub.Stub(x => x.GetCategoriesCount()).IgnoreArguments().Return(0);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();
            
            var cut = new CategoriesController(categoriesApiServiceStub, jsonFieldsSerializer);

            // act
            IHttpActionResult result = cut.GetCategoriesCount(categoriesCountParametersModel);

            // assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<CategoriesCountRootObject>>(result);
            Assert.AreEqual(0, ((OkNegotiatedContentResult<CategoriesCountRootObject>)result).Content.Count);
        }

        [Test]
        public void WhenSingleCategoryExists_ShouldReturnOKWithCountEqualToOne()
        {
            var categoriesCountParametersModel = new CategoriesCountParametersModel();

            // arrange
            var categoriesApiServiceStub = MockRepository.GenerateStub<ICategoryApiService>();
            categoriesApiServiceStub.Stub(x => x.GetCategoriesCount()).IgnoreArguments().Return(1);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();
            
            var cut = new CategoriesController(categoriesApiServiceStub, jsonFieldsSerializer);

            // act
            IHttpActionResult result = cut.GetCategoriesCount(categoriesCountParametersModel);

            // assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<CategoriesCountRootObject>>(result);
            Assert.AreEqual(1, ((OkNegotiatedContentResult<CategoriesCountRootObject>)result).Content.Count);
        }

        [Test]
        public void WhenCertainNumberOfCategoriesExist_ShouldReturnOKWithCountEqualToSameNumberOfCategories()
        {
            var categoriesCountParametersModel = new CategoriesCountParametersModel();
            int categoriesCount = 20;

            // arrange
            var categoriesApiServiceStub = MockRepository.GenerateStub<ICategoryApiService>();
            categoriesApiServiceStub.Stub(x => x.GetCategoriesCount()).IgnoreArguments().Return(categoriesCount);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();
            
            var cut = new CategoriesController(categoriesApiServiceStub, jsonFieldsSerializer);

            // act
            IHttpActionResult result = cut.GetCategoriesCount(categoriesCountParametersModel);

            // assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<CategoriesCountRootObject>>(result);
            Assert.AreEqual(categoriesCount, ((OkNegotiatedContentResult<CategoriesCountRootObject>)result).Content.Count);
        }
    }
}