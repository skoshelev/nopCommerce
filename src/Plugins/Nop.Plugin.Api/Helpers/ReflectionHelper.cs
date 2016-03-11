using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Web.Http.Results;
using Nop.Plugin.Api.ContractResolvers;

namespace Nop.Plugin.Api.Helpers
{
    public static class ReflectionHelper
    {
        public static bool HasProperty(string propertyName, Type type)
        {
            return type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null;
        }
        
        public static IHttpActionResult SerializeSpecificPropertiesOnly(object dto, Dictionary<string, bool> propertiesToSerialize, HttpRequestMessage request)
        {
            DynamicContractResolver dynamicContractResolver = new DynamicContractResolver(propertiesToSerialize);

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(dto,
                Newtonsoft.Json.Formatting.Indented,
                new Newtonsoft.Json.JsonSerializerSettings { ContractResolver = dynamicContractResolver });

            var response = request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return new ResponseMessageResult(response);
        }
        
        public static Dictionary<string, bool> GetPropertiesToSerialize(string fields)
        {
            var propertiesToSerialize = fields.ToLowerInvariant()
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Distinct()
                .ToDictionary(x => x, y => true);

            return propertiesToSerialize;
        }
    }
}