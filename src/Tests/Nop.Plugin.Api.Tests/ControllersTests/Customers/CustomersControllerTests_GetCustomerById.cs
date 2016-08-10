using System.Web.Http;
using System.Web.Http.Results;
using AutoMock;
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
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            autoMocker.Get<ICustomerApiService>().Stub(x => x.GetCustomerById(nonExistingCustomerId)).Return(null);

            // Act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetCustomerById(nonExistingCustomerId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldReturn404NotFound(int nonPositiveCustomerId)
        {
            // Arange
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            // Act
            IHttpActionResult result = autoMocker.ClassUnderTest.GetCustomerById(nonPositiveCustomerId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-20)]
        public void WhenIdEqualsToZeroOrLess_ShouldNotCallCustomerApiService(int negativeCustomerId)
        {
            // Arange
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            // Act
            autoMocker.ClassUnderTest.GetCustomerById(negativeCustomerId);

            // Assert
            autoMocker.Get<ICustomerApiService>().AssertWasNotCalled(x => x.GetCustomerById(negativeCustomerId));
        }

        [Test]
        public void WhenIdEqualsToExistingCustomerId_ShouldSerializeThatCustomer()
        {
            int existingCustomerId = 5;
            CustomerDto existingCustomerDto = new CustomerDto() { Id = existingCustomerId.ToString() };
           
            // Arange
            var autoMocker = new RhinoAutoMocker<CustomersController>();

            autoMocker.Get<ICustomerApiService>().Stub(x => x.GetCustomerById(existingCustomerId)).Return(existingCustomerDto);

            // Act
            autoMocker.ClassUnderTest.GetCustomerById(existingCustomerId);

            // Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(
                    Arg<CustomersRootObject>.Matches(objectToSerialize => objectToSerialize.Customers[0] == existingCustomerDto),
                Arg<string>.Matches(fields => fields == "")));
        }

        [Test]
        public void WhenIdEqualsToExistingCustomerIdAndFieldsSet_ShouldReturnJsonForThatCustomerWithSpecifiedFields()
        {
            int existingCustomerId = 5;
            CustomerDto existingCustomerDto = new CustomerDto() { Id = existingCustomerId.ToString() };
            string fields = "id,email";

            // Arange
            var autoMocker = new RhinoAutoMocker<CustomersController>();
            autoMocker.Get<ICustomerApiService>().Stub(x => x.GetCustomerById(existingCustomerId)).Return(existingCustomerDto);

            // Act
            autoMocker.ClassUnderTest.GetCustomerById(existingCustomerId, fields);

            // Assert
            autoMocker.Get<IJsonFieldsSerializer>().AssertWasCalled(
                x => x.Serialize(
                    Arg<CustomersRootObject>.Matches(objectToSerialize => objectToSerialize.Customers[0] == existingCustomerDto),
                Arg<string>.Matches(fieldsParameter => fieldsParameter == fields)));
        }

    }
}