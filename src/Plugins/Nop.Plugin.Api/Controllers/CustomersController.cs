using System.Web.Http;
using System.Web.Http.Description;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.MVC;
using Nop.Plugin.Api.Services;
using Nop.Services.Common;
using Nop.Services.Customers;

namespace Nop.Plugin.Api.Controllers
{
    public class CustomersController : ApiController
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerApiService _customerApiService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStateProvinceApiService _stateProvinceApiService;
        private readonly ICountryApiService _countryApiService;

        public CustomersController(ICustomerService customerService, 
            ICustomerApiService customerApiService, 
            IGenericAttributeService genericAttributeService, 
            IStateProvinceApiService stateProvinceApiService, 
            ICountryApiService countryApiService)
        {
            _customerService = customerService;
            _customerApiService = customerApiService;
            _genericAttributeService = genericAttributeService;
            _stateProvinceApiService = stateProvinceApiService;
            _countryApiService = countryApiService;
        }

        [HttpGet]
        [ResponseType(typeof(CustomersRootObject))]
        public IHttpActionResult GetCustomers(byte limit = Configurations.DefaultLimit, int page = Configurations.DefaultPageValue, int since_id = 0, string fields = "", string created_at_min = "", string created_at_max = "")
        {
            return Ok();
        }

        [HttpGet]
        [ResponseType(typeof(CustomersCountRootObject))]
        public IHttpActionResult GetCustomersCount()
        {
            return Ok();
        }

        [HttpGet]
        [ResponseType(typeof(CustomersRootObject))]
        public IHttpActionResult GetCustomerById(int id, string fields = "")
        {
            return Ok();
        }

        [HttpGet]
        public IHttpActionResult Search(string order = "Id", string query = "", int page = Configurations.DefaultPageValue, byte limit = Configurations.DefaultLimit, string fields = "")
        {
            return Ok();
        }
    }
}