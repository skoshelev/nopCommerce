using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nop.Plugin.Api.Validators
{
    public class FieldsValidator : IFieldsValidator
    {
        private List<string> GetPropertiesIntoList(string fields)
        {
            var properties = fields.ToLowerInvariant()
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Distinct()
                .ToList();

            return properties;
        }

        public Dictionary<string, bool> GetValidFields(string fields, Type type)
        {
            var validFields = new Dictionary<string, bool>();
            List<string> fieldsAsList = GetPropertiesIntoList(fields); 
            
            foreach (var field in fieldsAsList)
            {
                bool propertyExists = type.GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null;

                if (propertyExists)
                {
                    validFields.Add(field, true);
                }
            }

            return validFields;
        }
    }
}