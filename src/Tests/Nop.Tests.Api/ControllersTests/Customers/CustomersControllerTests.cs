using System.Web.Http;
using System.Web.Http.Results;
using AutoMapper;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.Controllers;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.Models.CustomersParameters;
using Nop.Plugin.Api.Services;
using Nop.Services.Customers;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Plugin.Api.Tests.ControllersTests.Customers
{
    [TestFixture]
    public class CustomersControllerTests
    {
        private CustomersController _customersController;

        [SetUp]
        public void SetUp()
        {
            var customersApiServiceStub = MockRepository.GenerateStub<ICustomerApiService>();
            var customersServiceStub = MockRepository.GenerateStub<ICustomerService>();

            customersServiceStub.Stub(x => x.GetCustomerById(1)).Return(new Customer() {});

            _customersController = new CustomersController(customersServiceStub, customersApiServiceStub);

            // Needed because the tests don't invoke the dependency register and the type maps are never registered.
            Mapper.CreateMap<Customer, CustomerDto>();
        }

        [Test]
        public void Get_customers_with_default_parameters()
        {
            IHttpActionResult response = _customersController.GetCustomers(new CustomersParametersModel());

            //var contentResult = response as OkNegotiatedContentResult<CustomersRootObject>;

            Assert.IsInstanceOf<OkNegotiatedContentResult<CustomersRootObject>>(response);
        }

        [Test]
        public void Get_customers_with_wrong_parameteres()
        {
            IHttpActionResult response = _customersController.GetCustomers(new CustomersParametersModel()
            {
                Page = -1
            });

            //var contentResult = response as OkNegotiatedContentResult<CustomersRootObject>;

            Assert.IsInstanceOf<BadRequestErrorMessageResult>(response);
        }

        [Test]
        public void Get_customer_by_id_with_valid_parameters()
        {
            IHttpActionResult response = _customersController.GetCustomerById(1);

            //var contentResult = response as OkNegotiatedContentResult<CustomersRootObject>;

            Assert.IsInstanceOf<OkNegotiatedContentResult<CustomersRootObject>>(response);
        }

        [Test]
        public void Get_customer_by_id_with_wrong_parameters()
        {
            IHttpActionResult response = _customersController.GetCustomerById(-1);

            //var contentResult = response as OkNegotiatedContentResult<CustomersRootObject>;

            Assert.IsInstanceOf<BadRequestErrorMessageResult>(response);
        }

        [Test]
        public void Get_customers_count()
        {
            IHttpActionResult response = _customersController.GetCustomersCount();

            //var contentResult = response as OkNegotiatedContentResult<CustomersRootObject>;

            Assert.IsInstanceOf<OkNegotiatedContentResult<CustomersCountRootObject>>(response);
        }
    }
}