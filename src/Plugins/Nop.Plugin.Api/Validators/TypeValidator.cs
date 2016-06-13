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
                bool isCurrentPropertyValid = true;

                if (jsonPropertyNameTypePair.ContainsKey(pair.Key))
                {
                    var propertyType = jsonPropertyNameTypePair[pair.Key];

                    // handle nested properties
                    if (pair.Value is Dictionary<string, object>)
                    {
                        Type constructedType = typeof (TypeValidator<>).MakeGenericType(propertyType);
                        var typeValidatorForNestedProperty = Activator.CreateInstance(constructedType);

                        var isValidMethod = constructedType.GetMethod("IsValid");

                        isCurrentPropertyValid = (bool)isValidMethod.Invoke(typeValidatorForNestedProperty, new object[] {pair.Value});
                    }
                    else
                    {
                        TypeConverter converter = TypeDescriptor.GetConverter(jsonPropertyNameTypePair[pair.Key]);

                        if (!converter.IsValid(pair.Value)) isCurrentPropertyValid = false;
                    }

                    if (!isCurrentPropertyValid)
                    {
                        isValid = false;
                        InvalidProperties.Add(pair.Key);
                    }
                }
            }

            return isValid;
        }
    }
}