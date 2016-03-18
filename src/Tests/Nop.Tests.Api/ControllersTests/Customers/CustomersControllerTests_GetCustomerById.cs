using System.Web.Http;
using System.Web.Http.Results;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;
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

            CustomersController cut = new CustomersController(customerApiServiceStub, jsonFieldsSerializer);

            // Act
            IHttpActionResult result = cut.GetCustomerById(nonExistingCustomerId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldReturn404NotFound(int nonPositiveCustomerId)
        {
            // Arange
            ICustomerApiService customerApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();
            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();

            CustomersController cut = new CustomersController(customerApiServiceStub, jsonFieldsSerializer);

            // Act
            IHttpActionResult result = cut.GetCustomerById(nonPositiveCustomerId);

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

            CustomersController cut = new CustomersController(customerApiServiceMock, jsonFieldsSerializer);

            // Act
            cut.GetCustomerById(negativeCustomerId);

            // Assert
            customerApiServiceMock.AssertWasNotCalled(x => x.GetCustomerById(negativeCustomerId));
        }

        [Test]
        public void WhenIdEqualsToExistingCustomerId_ShouldSerializeThatCustomer()
        {
            int existingCustomerId = 5;
            CustomerDto existingCustomerDto = new CustomerDto() { Id = existingCustomerId };
           
            // Arange
            ICustomerApiService customerApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();
            customerApiServiceStub.Stub(x => x.GetCustomerById(existingCustomerId)).Return(existingCustomerDto);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateMock<IJsonFieldsSerializer>();

            CustomersController cut = new CustomersController(customerApiServiceStub, jsonFieldsSerializer);

            // Act
            cut.GetCustomerById(existingCustomerId);

            // Assert
            jsonFieldsSerializer.AssertWasCalled(
                x => x.Serialize(
                    Arg<CustomersRootObject>.Matches(objectToSerialize => objectToSerialize.Customers[0] == existingCustomerDto),
                Arg<string>.Matches(fields => fields == "")));
        }

        [Test]
        public void WhenIdEqualsToExistingCustomerIdAndFieldsSet_ShouldReturnJsonForThatCustomerWithSpecifiedFields()
        {
            int existingCustomerId = 5;
            CustomerDto existingCustomerDto = new CustomerDto() { Id = existingCustomerId };
            string fields = "id,email";

            // Arange
            ICustomerApiService customerApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();
            customerApiServiceStub.Stub(x => x.GetCustomerById(existingCustomerId)).Return(existingCustomerDto);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateMock<IJsonFieldsSerializer>();

            CustomersController cut = new CustomersController(customerApiServiceStub, jsonFieldsSerializer);

            // Act
            cut.GetCustomerById(existingCustomerId, fields);

            // Assert
            jsonFieldsSerializer.AssertWasCalled(
                x => x.Serialize(
                    Arg<CustomersRootObject>.Matches(objectToSerialize => objectToSerialize.Customers[0] == existingCustomerDto),
                Arg<string>.Matches(fieldsParameter => fieldsParameter == fields)));
        }

    }
}