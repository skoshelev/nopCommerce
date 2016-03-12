using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Description;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.Helpers;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Models.CustomersParameters;
using Nop.Plugin.Api.MVC;
using Nop.Plugin.Api.Services;
using Nop.Services.Customers;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class CustomersController : ApiController
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerApiService _customerApiService;

        public CustomersController(ICustomerService customerService, 
            ICustomerApiService customerApiService)
        {
            _customerService = customerService;
            _customerApiService = customerApiService;
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

            if (!String.IsNullOrEmpty(parameters.Fields))
            {
                var propertiesToSerialize = ReflectionHelper.GetPropertiesToSerialize(parameters.Fields);
                if (!propertiesToSerialize.ContainsKey("customers"))
                {
                    propertiesToSerialize.Add("customers", true);
                }

                return ReflectionHelper.SerializeSpecificPropertiesOnly(customersRootObject, propertiesToSerialize, Request);
            }

            return Ok(customersRootObject);
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
        [ResponseType(typeof(CustomersRootObject))]
        public IHttpActionResult GetCustomerById(int id, string fields = "")
        {
            if (id <= 0)
            {
                return BadRequest("Invalid request parameters");
            }

            Customer customer = _customerService.GetCustomerById(id);

            var customersRootObject = new CustomersRootObject();

            if (customer != null)
            {
                customersRootObject.Customers.Add(customer.ToDto());
            }

            if (!String.IsNullOrEmpty(fields))
            {
                var propertiesToSerialize = ReflectionHelper.GetPropertiesToSerialize(fields);
                if (!propertiesToSerialize.ContainsKey("customers"))
                {
                    propertiesToSerialize.Add("customers", true);
                }

                return ReflectionHelper.SerializeSpecificPropertiesOnly(customersRootObject, propertiesToSerialize, Request);
            }

            return Ok(customersRootObject);
        }

        [HttpGet]
        public IHttpActionResult Search(CustomersSearchParametersModel parameters)
        {
            Dictionary<string, string> parsedQuery = EnsureSearchQueryIsValid(parameters.Query, ParseSearchQuery);

            if (parameters.Page <= 0)
            {
                return BadRequest("Invalid request parameters");
            }

            IList<CustomerDto> customerDto = _customerApiService.Search(parsedQuery, parameters.Order, parameters.Page, parameters.Limit).ToList();

            var customersRootObject = new CustomersRootObject()
            {
                Customers = customerDto
            };

            if (!String.IsNullOrEmpty(parameters.Fields))
            {
                var propertiesToSerialize = ReflectionHelper.GetPropertiesToSerialize(parameters.Fields);
                if (!propertiesToSerialize.ContainsKey("customers"))
                {
                    propertiesToSerialize.Add("customers", true);
                }

                return ReflectionHelper.SerializeSpecificPropertiesOnly(customersRootObject, propertiesToSerialize, Request);
            }

            return Ok(customersRootObject);
        }
        
        [NonAction]
        private Dictionary<string, string> EnsureSearchQueryIsValid(string query, Func<string, Dictionary<string, string>> parseSearchQuery)
        {
            if (!string.IsNullOrEmpty(query))
            {
                return parseSearchQuery(query);
            }

            return null;
        }

        [NonAction]
        private Dictionary<string, string> ParseSearchQuery(string query)
        {
            var parsedQuery = new Dictionary<string, string>();

            string splitPattern = @"(\w+):";

            var fieldValueList = Regex.Split(query, splitPattern).Where(s => s != String.Empty).ToList();

            if (fieldValueList.Count < 2)
            {
                return parsedQuery;
            }

            for (int i = 0; i < fieldValueList.Count; i += 2)
            {
                var field = fieldValueList[i];
                var value = fieldValueList[i + 1];

                if (!string.IsNullOrEmpty(field) && !string.IsNullOrEmpty(value))
                {
                    field = field.Replace("_", string.Empty);
                    parsedQuery.Add(field.Trim(), value.Trim());
                }
            }

            return parsedQuery;
        }
    }
}