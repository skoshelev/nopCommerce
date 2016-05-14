using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Plugin.Api.ContractResolvers;
using Nop.Plugin.Api.DTOs.Customers;
using Nop.Plugin.Api.Validators;

namespace Nop.Plugin.Api.Serializers
{
    public class JsonFieldsSerializer : IJsonFieldsSerializer
    {
        private const string DateTimeIso8601Format = "yyyy-MM-ddTHH:mm:sszzz";

        private readonly IFieldsValidator _fieldsValidator;

        public JsonFieldsSerializer(IFieldsValidator fieldsValidator)
        {
            _fieldsValidator = fieldsValidator;
        }

        public string Serialize(ISerializableObject objectToSerialize, string fields)
        {
            if (objectToSerialize == null)
            {
                throw new ArgumentNullException("objectToSerialize");
            }

            string json;

            if (!string.IsNullOrEmpty(fields))
            {
                string primaryPropertyName = objectToSerialize.GetPrimaryPropertyName();
                Dictionary<string, bool> validFields = _fieldsValidator.GetValidFields(fields, objectToSerialize.GetPrimaryPropertyType());

                // Always add the root manually
                validFields.Add(primaryPropertyName, true);

                json = Serialize(objectToSerialize, validFields);
            }
            else
            {
                json = Serialize(objectToSerialize);
            }

            return json;
        }

        private string Serialize(object objectToSerialize, Dictionary<string, bool> fieldsToSerialize)
        {
            DynamicContractResolver dynamicContractResolver = new DynamicContractResolver(fieldsToSerialize);

            string json = JsonConvert.SerializeObject(objectToSerialize,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    ContractResolver = dynamicContractResolver,
                    DateFormatString = DateTimeIso8601Format
                });

            //var serializer = new JsonSerializer();
            //serializer.Formatting = Formatting.Indented;
            //serializer.ContractResolver = dynamicContractResolver;
            //serializer.DateFormatString = DateTimeIso8601Format;
            //return JToken.FromObject(objectToSerialize, serializer).RemoveEmptyChildren().ToString();

            return json;
        }

        private string Serialize(object objectToSerialize)
        {
            string json = JsonConvert.SerializeObject(objectToSerialize,
                Formatting.Indented,
                new JsonSerializerSettings() { DateFormatString = DateTimeIso8601Format });

            return json;
        }
    }
}
