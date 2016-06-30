using System.Web.Http;
using System.Web.Http.Results;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.ProductCategoryMappings;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.ProductCategoryMappings
{
    [TestFixture]
    public class ProductCategoryMappingsControllerTests_GetMappingById
    {
        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldReturn404NotFound(int nonPositiveProductCategoryMappingId)
        {
            // Arange
            IProductCategoryMappingsApiService productCategoryMappingApiServiceStub = MockRepository.GenerateStub<IProductCategoryMappingsApiService>();
            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new ProductCategoryMappingsController(productCategoryMappingApiServiceStub, jsonFieldsSerializer);

            // Act
            IHttpActionResult result = cut.GetMappingById(nonPositiveProductCategoryMappingId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldNotCallProductCategoryMappingsApiService(int nonPositiveProductCategoryMappingId)
        {
            // Arange
            IProductCategoryMappingsApiService productCategoryMappingApiServiceStub = MockRepository.GenerateStub<IProductCategoryMappingsApiService>();

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();
            jsonFieldsSerializer.Stub(x => x.Serialize(null, null)).Return(string.Empty);

            var cut = new ProductCategoryMappingsController(productCategoryMappingApiServiceStub, jsonFieldsSerializer);

            // Act
            cut.GetMappingById(nonPositiveProductCategoryMappingId);

            // Assert
            productCategoryMappingApiServiceStub.AssertWasNotCalled(x => x.GetById(nonPositiveProductCategoryMappingId));
        }

        [Test]
        public void WhenIdIsPositiveNumberButNoSuchMappingExists_ShouldReturn404NotFound()
        {
            int nonExistingMappingId = 5;

            // Arange
            IProductCategoryMappingsApiService productCategoryMappingApiServiceStub = MockRepository.GenerateStub<IProductCategoryMappingsApiService>();
            productCategoryMappingApiServiceStub.Stub(x => x.GetById(nonExistingMappingId)).Return(null);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var cut = new ProductCategoryMappingsController(productCategoryMappingApiServiceStub, jsonFieldsSerializer);

            // Act
            IHttpActionResult result = cut.GetMappingById(nonExistingMappingId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public void WhenIdEqualsToExistingMappingId_ShouldSerializeThatMapping()
        {
            Maps.CreateMap<ProductCategory, ProductCategoryMappingDto>();

            int existingMappingId = 5;
            var existingMapping = new ProductCategory() { Id = existingMappingId };

            // Arange
            IProductCategoryMappingsApiService productCategoryMappingApiServiceStub = MockRepository.GenerateStub<IProductCategoryMappingsApiService>();
            productCategoryMappingApiServiceStub.Stub(x => x.GetById(existingMappingId)).Return(existingMapping);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateMock<IJsonFieldsSerializer>();

            var cut = new ProductCategoryMappingsController(productCategoryMappingApiServiceStub, jsonFieldsSerializer);

            // Act
            cut.GetMappingById(existingMappingId);

            // Assert
            jsonFieldsSerializer.AssertWasCalled(
                x => x.Serialize(
                    Arg<ProductCategoryMappingsRootObject>.Matches(
                        objectToSerialize =>
                               objectToSerialize.ProductCategoryMappingDtos.Count == 1 &&
                               objectToSerialize.ProductCategoryMappingDtos[0].Id == existingMapping.Id),
                    Arg<string>.Is.Equal("")));
        }

        [Test]
        public void WhenIdEqualsToExistingProductCategoryMappingIdAndFieldsSet_ShouldReturnJsonForThatProductCategoryMappingWithSpecifiedFields()
        {
            Maps.CreateMap<ProductCategory, ProductCategoryMappingDto>();

            int existingMappingId = 5;
            var existingMapping = new ProductCategory() { Id = existingMappingId };
            string fields = "id,name";

            // Arange
            IProductCategoryMappingsApiService productCategoryMappingApiServiceStub = MockRepository.GenerateStub<IProductCategoryMappingsApiService>();
            productCategoryMappingApiServiceStub.Stub(x => x.GetById(existingMappingId)).Return(existingMapping);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateMock<IJsonFieldsSerializer>();

            var cut = new ProductCategoryMappingsController(productCategoryMappingApiServiceStub, jsonFieldsSerializer);

            // Act
            cut.GetMappingById(existingMappingId, fields);

            // Assert
            jsonFieldsSerializer.AssertWasCalled(
                x => x.Serialize(
                    Arg<ProductCategoryMappingsRootObject>.Matches(objectToSerialize => objectToSerialize.ProductCategoryMappingDtos[0].Id == existingMapping.Id),
                    Arg<string>.Matches(fieldsParameter => fieldsParameter == fields)));
        }
    }
}