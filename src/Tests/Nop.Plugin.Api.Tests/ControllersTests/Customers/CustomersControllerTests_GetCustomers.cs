using System;
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
    public class CustomersControllerTests_GetCustomers
    {
        [Test]
        public void WhenSomeValidParametersPassed_ShouldCallTheServiceWithTheSameParameters()
        {
            var parameters = new CustomersParametersModel()
            {
                SinceId = Configurations.DefaultSinceId + 1, // some different than default since id
                CreatedAtMin = DateTime.Now,
                CreatedAtMax = DateTime.Now,
                Page = Configurations.DefaultPageValue + 1, // some different than default page
                Limit = Configurations.MinLimit + 1 // some different than default limit
            };

            //Arange
            ICustomerApiService customerApiServiceMock = MockRepository.GenerateMock<ICustomerApiService>();

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            CustomersController cut = new CustomersController(customerApiServiceMock, jsonFieldsSerializer);

            //Act
            cut.GetCustomers(parameters);

            //Assert
            customerApiServiceMock.AssertWasCalled(x => x.GetCustomersDtos(parameters.CreatedAtMin,
                                                    parameters.CreatedAtMax,
                                                    parameters.Limit,
                                                    parameters.Page,
                                                    parameters.SinceId));
        }

        [Test]
        public void WhenSomeCustomersExist_ShouldCallTheSerializerWithTheseCustomers()
        {
            var returnedCustomersDtoCollection = new List<CustomerDto>()
            {
                new CustomerDto(),
                new CustomerDto()
            };

            var parameters = new CustomersParametersModel();

            //Arange
            ICustomerApiService customerApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();
            customerApiServiceStub.Stub(x => x.GetCustomersDtos()).Return(returnedCustomersDtoCollection);

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();


            CustomersController cut = new CustomersController(customerApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.GetCustomers(parameters);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(
                x => x.Serialize(Arg<CustomersRootObject>.Matches(r => r.Customers.Count == returnedCustomersDtoCollection.Count),
                Arg<string>.Is.Equal(parameters.Fields)));
        }

        [Test]
        public void WhenNoCustomersExist_ShouldCallTheSerializerWithNoCustomers()
        {
            var returnedCustomersDtoCollection = new List<CustomerDto>();

            var parameters = new CustomersParametersModel();

            //Arange
            ICustomerApiService customerApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();
            customerApiServiceStub.Stub(x => x.GetCustomersDtos()).Return(returnedCustomersDtoCollection);

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();


            CustomersController cut = new CustomersController(customerApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.GetCustomers(parameters);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(
                x => x.Serialize(Arg<CustomersRootObject>.Matches(r => r.Customers.Count == returnedCustomersDtoCollection.Count),
                Arg<string>.Is.Equal(parameters.Fields)));
        }

        [Test]
        public void WhenFieldsParametersPassed_ShouldCallTheSerializerWithTheSameFields()
        {
            var parameters = new CustomersParametersModel()
            {
                Fields = "id,email"
            };

            //Arange
            ICustomerApiService customerApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();

            CustomersController cut = new CustomersController(customerApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.GetCustomers(parameters);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(
                x => x.Serialize(Arg<CustomersRootObject>.Is.Anything, Arg<string>.Is.Equal(parameters.Fields)));
        }

        [Test]
        [TestCase(Configurations.MinLimit - 1)]
        [TestCase(Configurations.MaxLimit + 1)]
        public void WhenInvalidLimitParameterPassed_ShouldReturnBadRequest(int invalidLimit)
        {
            var parameters = new CustomersParametersModel()
            {
                Limit = invalidLimit
            };

            //Arange
            ICustomerApiService customerApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();

            IJsonFieldsSerializer jsonFieldsSerializerStub = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            CustomersController cut = new CustomersController(customerApiServiceStub, jsonFieldsSerializerStub);

            //Act
            IHttpActionResult result = cut.GetCustomers(parameters);

            //Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public void WhenInvalidPageParameterPassed_ShouldReturnBadRequest(int invalidPage)
        {
            var parameters = new CustomersParametersModel()
            {
                Page = invalidPage
            };

            //Arange
            ICustomerApiService customerApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();

            IJsonFieldsSerializer jsonFieldsSerializerStub = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            CustomersController cut = new CustomersController(customerApiServiceStub, jsonFieldsSerializerStub);

            //Act
            IHttpActionResult result = cut.GetCustomers(parameters);

            //Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
        }
    }
}