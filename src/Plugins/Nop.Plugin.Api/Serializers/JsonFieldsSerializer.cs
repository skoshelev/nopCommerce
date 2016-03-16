using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Plugin.Api.ContractResolvers;

namespace Nop.Plugin.Api.Serializers
{
    public class JsonFieldsSerializer : IJsonFieldsSerializer
    {
        private Dictionary<string, bool> GetPropertiesToSerialize(string fields)
        {
            var propertiesToSerialize = fields.ToLowerInvariant()
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Distinct()
                .ToDictionary(x => x, y => true);

            return propertiesToSerialize;
        }

        public string Serialize(object objectToSerialize, string fieldsToSerialize)
        {
            DynamicContractResolver dynamicContractResolver = new DynamicContractResolver(GetPropertiesToSerialize(fieldsToSerialize));

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(objectToSerialize,
                Newtonsoft.Json.Formatting.Indented,
                new Newtonsoft.Json.JsonSerializerSettings { ContractResolver = dynamicContractResolver });

            return json;
        }
    }
}
