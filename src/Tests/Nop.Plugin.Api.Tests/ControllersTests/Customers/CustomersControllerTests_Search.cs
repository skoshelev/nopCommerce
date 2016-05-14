using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;
using Nop.Plugin.Api.Constants;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.Models.CustomersParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Customers
{
    [TestFixture]
    public class CustomersControllerTests_Search
    {
        [Test]
        public void WhenNoParametersPassed_ShouldCallTheServiceWithDefaultParameters()
        {
            var defaultParametersModel = new CustomersSearchParametersModel();

            //Arange
            ICustomerApiService customerApiServiceMock = MockRepository.GenerateMock<ICustomerApiService>();
           
            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            CustomersController cut = new CustomersController(customerApiServiceMock, jsonFieldsSerializer);

            //Act
            cut.Search(defaultParametersModel);

            //Assert
            customerApiServiceMock.AssertWasCalled(x => x.Search(defaultParametersModel.Query,
                                                    defaultParametersModel.Order,
                                                    defaultParametersModel.Page,
                                                    defaultParametersModel.Limit));
        }

        [Test]
        public void WhenNoParametersPassedAndSomeCustomersExist_ShouldCallTheSerializer()
        {
            var expectedCustomersCollection = new List<CustomerDto>()
            {
                new CustomerDto(),
                new CustomerDto()
            };

            var expectedRootObject = new CustomersRootObject()
            {
                Customers = expectedCustomersCollection
            };

            var defaultParameters = new CustomersSearchParametersModel();

            //Arange
            ICustomerApiService customerApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();
            customerApiServiceStub.Stub(x => x.Search()).Return(expectedCustomersCollection);

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();
            jsonFieldsSerializerMock.Expect(x => x.Serialize(expectedRootObject, defaultParameters.Fields));

            CustomersController cut = new CustomersController(customerApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.Search(defaultParameters);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(x => x.Serialize(Arg<CustomersRootObject>.Is.TypeOf,
                Arg<string>.Is.Equal(defaultParameters.Fields)));
        }

        [Test]
        public void WhenNoParametersPassedAndNoCustomersExist_ShouldCallTheSerializer()
        {
            var expectedCustomersCollection = new List<CustomerDto>();

            var expectedRootObject = new CustomersRootObject()
            {
                Customers = expectedCustomersCollection
            };

            var defaultParameters = new CustomersSearchParametersModel();

            //Arange
            ICustomerApiService customerApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();
            customerApiServiceStub.Stub(x => x.Search()).Return(expectedCustomersCollection);

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();
            jsonFieldsSerializerMock.Expect(x => x.Serialize(expectedRootObject, defaultParameters.Fields));

            CustomersController cut = new CustomersController(customerApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.Search(defaultParameters);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(x => x.Serialize(Arg<CustomersRootObject>.Is.TypeOf,
                Arg<string>.Is.Equal(defaultParameters.Fields)));
        }

        [Test]
        public void WhenFieldsParametersPassed_ShouldCallTheSerializerWithTheSameFields()
        {
            var defaultParametersModel = new CustomersSearchParametersModel()
            {
                Fields = "id,email"
            };

            //Arange
            ICustomerApiService customerApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();

            CustomersController cut = new CustomersController(customerApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.Search(defaultParametersModel);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(
                x => x.Serialize(Arg<CustomersRootObject>.Is.Anything, Arg<string>.Is.Equal(defaultParametersModel.Fields)));
        }

        [Test]
        [TestCase(Configurations.MinLimit)]
        [TestCase(Configurations.MinLimit - 1)]
        [TestCase(Configurations.MaxLimit + 1)]
        public void WhenInvalidLimitPassed_ShouldReturnBadRequest(int invalidLimit)
        {
            var parametersModel = new CustomersSearchParametersModel()
            {
                Limit = invalidLimit
            };

            //Arange
            ICustomerApiService customerApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();

            IJsonFieldsSerializer jsonFieldsSerializerStub = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            CustomersController cut = new CustomersController(customerApiServiceStub, jsonFieldsSerializerStub);

            //Act
            IHttpActionResult result = cut.Search(parametersModel);

            //Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public void WhenNonPositivePagePassed_ShouldReturnBadRequest(int nonPositivePage)
        {
            var parametersModel = new CustomersSearchParametersModel()
            {
                Limit = nonPositivePage
            };

            //Arange
            ICustomerApiService customerApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();

            IJsonFieldsSerializer jsonFieldsSerializerStub = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            CustomersController cut = new CustomersController(customerApiServiceStub, jsonFieldsSerializerStub);

            //Act
            IHttpActionResult result = cut.Search(parametersModel);

            //Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
        }
    }
}