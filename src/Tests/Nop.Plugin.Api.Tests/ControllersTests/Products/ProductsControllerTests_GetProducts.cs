using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.Models.ProductsParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Plugin.Api.Validators;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Products
{
    [TestFixture]
    public class ProductsControllerTests_GetProducts
    {
        [Test]
        [TestCase("bbb")]
        [TestCase(",,,,")]
        [TestCase("asd,asda")]
        [TestCase("1234323232323223")]
        public void WhenNonNumericIdsParameterPassed_ShouldCallTheServiceWithNullIds(string ids)
        {
            var parametersModel = new ProductsParametersModel()
            {
                Ids = ids
            };

            //Arange
            IProductApiService productApiServiceMock = MockRepository.GenerateMock<IProductApiService>();

            productApiServiceMock.Expect(x => x.GetProducts(null,
                                                    parametersModel.CreatedAtMin,
                                                    parametersModel.CreatedAtMax,
                                                    parametersModel.UpdatedAtMin,
                                                    parametersModel.UpdatedAtMax,
                                                    parametersModel.Limit,
                                                    parametersModel.Page,
                                                    parametersModel.SinceId,
                                                    parametersModel.CategoryId,
                                                    parametersModel.VendorName,
                                                    parametersModel.PublishedStatus)).Return(new List<Product>());

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var parametersValidator = new ParametersValidator();

            var cut = new ProductsController(productApiServiceMock, jsonFieldsSerializer, parametersValidator);

            //Act
            cut.GetProducts(parametersModel);

            //Assert
            productApiServiceMock.VerifyAllExpectations();
        }

        [Test]
        [TestCase("1")]
        [TestCase("1,1")]
        [TestCase("1,sasa")]
        [TestCase("asda,1,sasa,aa")]
        public void WhenSigleValidNumericIdParameterPassed_ShouldCallTheServiceWithThatId(string ids)
        {
            var parametersModel = new ProductsParametersModel()
            {
                Ids = ids
            };

            //Arange
            IProductApiService productApiServiceMock = MockRepository.GenerateMock<IProductApiService>();

            productApiServiceMock.Expect(x =>
                    x.GetProducts(Arg<IList<int>>.Matches(l => l.Contains(1) && l.Count == 1),
                    Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything,
                    Arg<int>.Is.Anything, Arg<int>.Is.Anything, Arg<int>.Is.Anything, Arg<int>.Is.Anything,
                    Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                        .Return(new List<Product>());

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            var parametersValidator = new ParametersValidator();

            var cut = new ProductsController(productApiServiceMock, jsonFieldsSerializer, parametersValidator);

            //Act
            cut.GetProducts(parametersModel);

            //Assert
            productApiServiceMock.VerifyAllExpectations();
        }
    }
}