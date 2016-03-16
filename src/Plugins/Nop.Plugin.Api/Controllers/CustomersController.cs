using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
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

            //TODO: Would be nice to have fields validator against the CustomerDTO properties
            // so that if none of the passed fields exist we could skip the serialization below.
            // One possible solution is to build the available CustomDTO properties into a dictionary
            // on startup and keep it in the static cache
            // If not valid we simply return an empty Customers object
            // new CustomersRootObject

            // if (!string.IsNullOrEmpty(fields))
            // {
            // bool validFields = Validator.TryParse(fields, out validFields);
            // if( !validFields )
            // {
            // return "{"customers" : {} }";
            //  } else
            // Code below

            var customersRootObject = new CustomersRootObject();
            customersRootObject.Customers.Add(customer);

            //// TODO: Remove this check after implementing the code above
            //if (!string.IsNullOrEmpty(fields))
            //{
                //TODO: Add "customers" to fields i.e fields = "customers," + fields
                string json = _jsonFieldsSerializer.Serialize(customersRootObject, "customers,"+fields);

               // return GetJsonResult(json);
            //}

            return new RawJsonActionResult(json);

            //return Ok(customersRootObject);
        }

        private IHttpActionResult GetJsonResult(string json)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return new ResponseMessageResult(response);
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