using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Customers;

namespace Nop.Plugin.Api.DTOs.Categories
{
    public class CategoriesRootObject : ISerializableObject
    {
        public CategoriesRootObject()
        {
            Categories = new List<CategoryDto>();
        }

        [JsonProperty("categories")]
        public IList<CategoryDto> Categories { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "categories";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof (CategoryDto);
        }
    }
}