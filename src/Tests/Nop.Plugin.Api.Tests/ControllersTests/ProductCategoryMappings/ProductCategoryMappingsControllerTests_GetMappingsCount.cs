using System.Web.Http;
using System.Web.Http.Results;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.ProductCategoryMappings;
using Nop.Plugin.Api.DTOs.Products;
using Nop.Plugin.Api.Models.ProductCategoryMappingsParameters;
using Nop.Plugin.Api.Models.ProductsParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.ProductCategoryMappings
{
    [TestFixture]
    public class ProductCategoryMappingsControllerTests_GetMappingsCount
    {
        [Test]
        public void WhenNoProductCategoryMappingsExist_ShouldReturnOKResultWithCountEqualToZero()
        {
            var parameters = new ProductCategoryMappingsCountParametersModel();

            // arrange
            var productCategoryMappingsApiServiceStub = MockRepository.GenerateStub<IProductCategoryMappingsApiService>();
            productCategoryMappingsApiServiceStub.Stub(x => x.GetMappingsCount()).IgnoreArguments().Return(0);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new ProductCategoryMappingsController(productCategoryMappingsApiServiceStub, jsonFieldsSerializer);

            // act
            IHttpActionResult result = cut.GetMappingsCount(parameters);

            // assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<ProductCategoryMappingsCountRootObject>>(result);
            Assert.AreEqual(0, ((OkNegotiatedContentResult<ProductCategoryMappingsCountRootObject>)result).Content.Count);
        }

        [Test]
        public void WhenSingleProductCategoryMappingExists_ShouldReturnOKWithCountEqualToOne()
        {
            var parameters = new ProductCategoryMappingsCountParametersModel();

            // arrange
            var productCategoryMappingsApiServiceStub = MockRepository.GenerateStub<IProductCategoryMappingsApiService>();
            productCategoryMappingsApiServiceStub.Stub(x => x.GetMappingsCount()).IgnoreArguments().Return(1);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new ProductCategoryMappingsController(productCategoryMappingsApiServiceStub, jsonFieldsSerializer);

            // act
            IHttpActionResult result = cut.GetMappingsCount(parameters);

            // assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<ProductCategoryMappingsCountRootObject>>(result);
            Assert.AreEqual(1, ((OkNegotiatedContentResult<ProductCategoryMappingsCountRootObject>)result).Content.Count);
        }

        [Test]
        public void WhenCertainNumberOfProductCategoryMappingsExist_ShouldReturnOKWithCountEqualToSameNumberOfProductCategoryMappings()
        {
            var mappingsCountParametersModel = new ProductCategoryMappingsCountParametersModel();
            int mappingsCount = 20;

            // arrange
            var productCategoryMappingsApiServiceStub = MockRepository.GenerateStub<IProductCategoryMappingsApiService>();
            productCategoryMappingsApiServiceStub.Stub(x => x.GetMappingsCount()).IgnoreArguments().Return(mappingsCount);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new ProductCategoryMappingsController(productCategoryMappingsApiServiceStub, jsonFieldsSerializer);

            // act
            IHttpActionResult result = cut.GetMappingsCount(mappingsCountParametersModel);

            // assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<ProductCategoryMappingsCountRootObject>>(result);
            Assert.AreEqual(mappingsCount, ((OkNegotiatedContentResult<ProductCategoryMappingsCountRootObject>)result).Content.Count);
        }
    }
}