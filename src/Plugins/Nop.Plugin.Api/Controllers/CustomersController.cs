using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Description;
using Nop.Plugin.Api.ActionResults;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.Helpers;
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

        [HttpGet]
        [ResponseType(typeof(CustomersRootObject))]
        public IHttpActionResult GetCustomers(CustomersParametersModel parameters)
        {
            if (parameters.Limit <= Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
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