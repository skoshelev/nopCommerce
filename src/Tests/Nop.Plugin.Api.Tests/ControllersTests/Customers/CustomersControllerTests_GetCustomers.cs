﻿using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.Models.CustomersParameters;
using Nop.Plugin.Api.MVC;
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
        public void WhenNoParametersPassed_ShouldCallTheServiceWithDefaultParameters()
        {
            var defaultParametersModel = new CustomersParametersModel();

            //Arange
            ICustomerApiService customerApiServiceMock = MockRepository.GenerateMock<ICustomerApiService>();
           
            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            CustomersController cut = new CustomersController(customerApiServiceMock, jsonFieldsSerializer);

            //Act
            cut.GetCustomers(defaultParametersModel);

            //Assert
            customerApiServiceMock.AssertWasCalled(x => x.GetCustomersDtos(defaultParametersModel.CreatedAtMin,
                                                    defaultParametersModel.CreatedAtMax,
                                                    defaultParametersModel.Limit,
                                                    defaultParametersModel.Page,
                                                    defaultParametersModel.SinceId));
        }

        [Test]
        public void WhenValidAndDifferentThanDefaultParametersPassed_ShouldCallTheServiceWithTheseParameters()
        {
            var parametersModel = new CustomersParametersModel()
            {
                SinceId = Configurations.DefaultSinceId + 1, // some different than default since id
                CreatedAtMin = "some valid date",
                CreatedAtMax = "some valid date",
                Page = Configurations.DefaultPageValue + 1, // some different than default page
                Limit = Configurations.MinLimit + 1 // some different than default limit
            };

            //Arange
            ICustomerApiService customerApiServiceMock = MockRepository.GenerateMock<ICustomerApiService>();

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            CustomersController cut = new CustomersController(customerApiServiceMock, jsonFieldsSerializer);

            //Act
            cut.GetCustomers(parametersModel);

            //Assert
            customerApiServiceMock.AssertWasCalled(x => x.GetCustomersDtos(parametersModel.CreatedAtMin,
                                                    parametersModel.CreatedAtMax,
                                                    parametersModel.Limit,
                                                    parametersModel.Page,
                                                    parametersModel.SinceId));
        }

        [Test]
        public void WhenNoParametersPassedAndSomeCustomersExist_ShouldCallTheSerializerWithTheseCustomers()
        {
            var returnedCustomersDtoCollection = new List<CustomerDto>()
            {
                new CustomerDto(),
                new CustomerDto()
            };

            var defaultParameters = new CustomersParametersModel();

            //Arange
            ICustomerApiService customerApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();
            customerApiServiceStub.Stub(x => x.GetCustomersDtos()).Return(returnedCustomersDtoCollection);

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();
            

            CustomersController cut = new CustomersController(customerApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.GetCustomers(defaultParameters);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(
                x => x.Serialize(Arg<CustomersRootObject>.Matches(r => r.Customers.Count == returnedCustomersDtoCollection.Count),
                Arg<string>.Is.Equal(defaultParameters.Fields)));
        }

        [Test]
        public void WhenNoParametersPassedAndNoCustomersExist_ShouldCallTheSerializerWithNoCustomers()
        {
            var returnedCustomersDtoCollection = new List<CustomerDto>();

            var defaultParameters = new CustomersParametersModel();

            //Arange
            ICustomerApiService customerApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();
            customerApiServiceStub.Stub(x => x.GetCustomersDtos()).Return(returnedCustomersDtoCollection);

            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();


            CustomersController cut = new CustomersController(customerApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.GetCustomers(defaultParameters);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(
                x => x.Serialize(Arg<CustomersRootObject>.Matches(r => r.Customers.Count == returnedCustomersDtoCollection.Count),
                Arg<string>.Is.Equal(defaultParameters.Fields)));
        }

        [Test]
        public void WhenFieldsParametersPassed_ShouldCallTheSerializerWithTheSameFields()
        {
            var defaultParametersModel = new CustomersParametersModel()
            {
                Fields = "id,email"
            };

            //Arange
            ICustomerApiService customerApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();
           
            IJsonFieldsSerializer jsonFieldsSerializerMock = MockRepository.GenerateMock<IJsonFieldsSerializer>();
           
            CustomersController cut = new CustomersController(customerApiServiceStub, jsonFieldsSerializerMock);

            //Act
            cut.GetCustomers(defaultParametersModel);

            //Assert
            jsonFieldsSerializerMock.AssertWasCalled(
                x => x.Serialize(Arg<CustomersRootObject>.Is.Anything, Arg<string>.Is.Equal(defaultParametersModel.Fields)));
        }

        [Test]
        [TestCase(Configurations.MinLimit - 1)]
        [TestCase(Configurations.MaxLimit + 1)]
        public void WhenInvalidLimitParameterPassed_ShouldReturnBadRequest(int invalidLimit)
        {
            var parametersModel = new CustomersParametersModel()
            {
                Limit = invalidLimit
            };

            //Arange
            ICustomerApiService customerApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();

            IJsonFieldsSerializer jsonFieldsSerializerStub = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            CustomersController cut = new CustomersController(customerApiServiceStub, jsonFieldsSerializerStub);

            //Act
            IHttpActionResult result = cut.GetCustomers(parametersModel);

            //Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
        }
        
        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        public void WhenInvalidPageParameterPassed_ShouldReturnBadRequest(int invalidPage)
        {
            var parametersModel = new CustomersParametersModel()
            {
                Limit = invalidPage
            };

            //Arange
            ICustomerApiService customerApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();

            IJsonFieldsSerializer jsonFieldsSerializerStub = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            CustomersController cut = new CustomersController(customerApiServiceStub, jsonFieldsSerializerStub);

            //Act
            IHttpActionResult result = cut.GetCustomers(parametersModel);

            //Assert
            Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
        }
    }
}