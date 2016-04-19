using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using Nop.Plugin.Api.ActionResults;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.Models.CustomersParameters;
using Nop.Plugin.Api.MVC;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class CustomersController : ApiController
    {
        private readonly ICustomerApiService _customerApiService;
        private readonly IJsonFieldsSerializer _jsonFieldsSerializer;

        public CustomersController(ICustomerApiService customerApiService, IJsonFieldsSerializer jsonFieldsSerializer)
        {
            _customerApiService = customerApiService;
            _jsonFieldsSerializer = jsonFieldsSerializer;
        }

        /// <summary>
        /// Retrieve all customers of a shop
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(CustomersRootObject))]
        public IHttpActionResult GetCustomers(CustomersParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return BadRequest("Invalid request parameters");
            }

            if (parameters.Page <= 0)
            {
                return BadRequest("Invalid request parameters");
            }

            IList<CustomerDto> allCustomers = _customerApiService.GetCustomersDtos(parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.Limit, parameters.Page, parameters.SinceId);

            var customersRootObject = new CustomersRootObject()
            {
                Customers = allCustomers
            };

            var json = _jsonFieldsSerializer.Serialize(customersRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Retrieve customer by spcified id
        /// </summary>
        /// <param name="id">Id of the customer</param>
        /// <param name="fields">Fields from the customer you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(CustomersRootObject))]
        public IHttpActionResult GetCustomerById(int id, string fields = "")
        {
            if (id <= 0)
            {
                return NotFound();
            }

            CustomerDto customer = _customerApiService.GetCustomerById(id);

            if (customer == null)
            {
                return NotFound();
            }
            
            var customersRootObject = new CustomersRootObject();
            customersRootObject.Customers.Add(customer);

            var json = _jsonFieldsSerializer.Serialize(customersRootObject, fields);

            return new RawJsonActionResult(json);
        }


        /// <summary>
        /// Get a count of all customers
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [ResponseType(typeof(CustomersCountRootObject))]
        public IHttpActionResult GetCustomersCount()
        {
            var allCustomersCount = _customerApiService.GetCustomersCount();

            var customersCountRootObject = new CustomersCountRootObject()
            {
                Count = allCustomersCount
            };

            return Ok(customersCountRootObject);
        }

        /// <summary>
        /// Search for customers matching supplied query
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        public IHttpActionResult Search(CustomersSearchParametersModel parameters)
        {
            if (parameters.Limit <= Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return BadRequest("Invalid request parameters");
            }

            if (parameters.Page <= 0)
            {
                return BadRequest("Invalid request parameters");
            }

            if (parameters.Page <= 0)
            {
                return BadRequest("Invalid request parameters");
            }

            IList<CustomerDto> customersDto = _customerApiService.Search(parameters.Query, parameters.Order, parameters.Page, parameters.Limit);

            var customersRootObject = new CustomersRootObject()
            {
                Customers = customersDto
            };

            var json = _jsonFieldsSerializer.Serialize(customersRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }
        
    }
}