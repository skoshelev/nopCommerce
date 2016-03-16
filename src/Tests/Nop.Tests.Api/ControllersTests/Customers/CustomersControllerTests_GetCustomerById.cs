using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using AutoMapper;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.Models.CustomersParameters;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
using Nop.Services.Customers;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Customers
{
    [TestFixture]
    public class CustomersControllerTests_GetCustomerById
    {
        [Test]
        public void WhenIdIsPositiveNumberButNoSuchCustmerWithSuchIdExists_ShouldReturn404NotFound()
        {
            int nonExistingCustomerId = 5;

            // Arange
            ICustomerApiService customerApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();
            customerApiServiceStub.Stub(x => x.GetCustomerById(nonExistingCustomerId)).Return(null);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();
            jsonFieldsSerializer.Stub(x => x.Serialize(null, null)).Return(string.Empty);

            CustomersController cut = new CustomersController(customerApiServiceStub, jsonFieldsSerializer);

            // Act
            IHttpActionResult result = cut.GetCustomerById(nonExistingCustomerId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
        
        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldReturn404NotFound(int negativeCustomerId)
        {
            // Arange
            ICustomerApiService customerApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();
            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();
            jsonFieldsSerializer.Stub(x => x.Serialize(null, null)).Return(string.Empty);

            CustomersController cut = new CustomersController(customerApiServiceStub,jsonFieldsSerializer);

            // Act
            IHttpActionResult result = cut.GetCustomerById(negativeCustomerId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldNotCallCustomerApiService(int negativeCustomerId)
        {
            // Arange
            ICustomerApiService customerApiServiceMock = MockRepository.GenerateMock<ICustomerApiService>();
            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();
            jsonFieldsSerializer.Stub(x => x.Serialize(null, null)).Return(string.Empty);

            CustomersController cut = new CustomersController(customerApiServiceMock,jsonFieldsSerializer);

            // Act
            cut.GetCustomerById(negativeCustomerId);

            // Assert
            customerApiServiceMock.AssertWasNotCalled(x => x.GetCustomerById(negativeCustomerId));
        }
        
        [Test]
        public void WhenIdEqualsToExistingCustomerId_ShouldReturnThatCustomer()
        {
            int existingCustomerId = 5;
            CustomerDto existingCustomerDto = new CustomerDto() { Id = existingCustomerId };

            // Arange
            ICustomerApiService customerApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();
            customerApiServiceStub.Stub(x => x.GetCustomerById(existingCustomerId)).Return(existingCustomerDto);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();
            
            CustomersController cut = new CustomersController(customerApiServiceStub,jsonFieldsSerializer);
            
            // Act
            IHttpActionResult result = cut.GetCustomerById(existingCustomerId);

            // Assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<CustomersRootObject>> (result);
            Assert.AreEqual(existingCustomerId, ((OkNegotiatedContentResult<CustomersRootObject>)result).Content.Customers.First().Id);
        }

        [Test]
        public void WhenIdEqualsToExistingCustomerIdAndFieldsSetToId_ShouldReturnJsonForThatCustomerWithOnlyId()
        {
            int existingCustomerId = 5;
            CustomerDto existingCustomerDto = new CustomerDto() { Id = existingCustomerId };
            string serializedJson = "some json";

            // Arange
            ICustomerApiService customerApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();
            customerApiServiceStub.Stub(x => x.GetCustomerById(existingCustomerId)).Return(existingCustomerDto);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();
            jsonFieldsSerializer.Stub(x => x.Serialize(null, null)).IgnoreArguments().Return(serializedJson);

            CustomersController cut = new CustomersController(customerApiServiceStub, jsonFieldsSerializer);
            cut.Request = new HttpRequestMessage();

            // Act
            IHttpActionResult result = cut.GetCustomerById(existingCustomerId, "id");

            // Assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<StringContent>>(result);
            //Task<string> task = 
            //    ((StringContent)(((ResponseMessageResult)result).Response.Content)).ReadAsStringAsync();
            
            Assert.AreEqual(serializedJson, ((OkNegotiatedContentResult<StringContent>)result).Content);
        }

    }
}