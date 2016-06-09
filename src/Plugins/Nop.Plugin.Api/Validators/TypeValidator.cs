using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Newtonsoft.Json;

namespace Nop.Plugin.Api.Validators
{
    public class TypeValidator<T>
    {
        public List<string> InvalidProperties { get; set; }

        public TypeValidator()
        {
            InvalidProperties = new List<string>();    
        }

        public bool IsValid(Dictionary<string, object> propertyValuePaires)
        {
            bool isValid = true;
            
            var jsonPropertyNameTypePair = new Dictionary<string, Type>();

            var objectProperties = typeof (T).GetProperties();

            foreach (var property in objectProperties)
            {
                JsonPropertyAttribute jsonPropertyAttribute = property.GetCustomAttribute(typeof(JsonPropertyAttribute)) as JsonPropertyAttribute;

                if (jsonPropertyAttribute != null)
                {
                    if (!jsonPropertyNameTypePair.ContainsKey(jsonPropertyAttribute.PropertyName))
                    {
                        jsonPropertyNameTypePair.Add(jsonPropertyAttribute.PropertyName, property.PropertyType);
                    }
                }
            }
            
            foreach (var pair in propertyValuePaires)
            {
                if (jsonPropertyNameTypePair.ContainsKey(pair.Key))
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(jsonPropertyNameTypePair[pair.Key]);

                    if (!converter.IsValid(pair.Value))
                    {
                        InvalidProperties.Add(pair.Key);
                        isValid = false;
                    }
                }
            }

            return isValid;
        }
    }
}