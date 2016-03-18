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
    public class CustomersControllerTests_GetCustomersCount
    {
        [Test]
        public void WhenNoCustomersExist_ShouldReturnOKResultWithCountEqualToZero()
        {
            // arrange
            var customersApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();
            customersApiServiceStub.Stub(x => x.GetCustomersCount()).Return(0);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();
            jsonFieldsSerializer.Stub(x => x.Serialize(null, null)).Return(string.Empty);

            CustomersController cut = new CustomersController(customersApiServiceStub,jsonFieldsSerializer);

            // act
            IHttpActionResult result = cut.GetCustomersCount();

            // assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<CustomersCountRootObject>>(result);
            Assert.AreEqual(0,((OkNegotiatedContentResult<CustomersCountRootObject>)result).Content.Count);
        }

        [Test]
        public void WhenSingleCustomerExists_ShouldReturnOKWithCountEqualToOne()
        {
            // arrange
            var customersApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();
            customersApiServiceStub.Stub(x => x.GetCustomersCount()).Return(1);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();
            jsonFieldsSerializer.Stub(x => x.Serialize(null, null)).Return(string.Empty);

            CustomersController cut = new CustomersController(customersApiServiceStub,jsonFieldsSerializer);

            // act
            IHttpActionResult result = cut.GetCustomersCount();

            // assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<CustomersCountRootObject>>(result);
            Assert.AreEqual(1, ((OkNegotiatedContentResult<CustomersCountRootObject>)result).Content.Count);
        }

        [Test]
        public void WhenCertainNumberOfCustomersExist_ShouldReturnOKWithCountEqualToSameNumberOfCustomers()
        {
            // arrange
            var customersApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();
            customersApiServiceStub.Stub(x => x.GetCustomersCount()).Return(20000);

            IJsonFieldsSerializer jsonFieldsSerializer = MockRepository.GenerateStub<IJsonFieldsSerializer>();
            jsonFieldsSerializer.Stub(x => x.Serialize(null, null)).Return(string.Empty);

            CustomersController cut = new CustomersController(customersApiServiceStub,jsonFieldsSerializer);

            // act
            IHttpActionResult result = cut.GetCustomersCount();
            
            // assert
            Assert.IsInstanceOf<OkNegotiatedContentResult<CustomersCountRootObject>>(result);
            Assert.AreEqual(20000, ((OkNegotiatedContentResult<CustomersCountRootObject>)result).Content.Count);
        }
    }
}