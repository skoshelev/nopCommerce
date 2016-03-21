using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.Customers;

namespace Nop.Plugin.Api.Tests.SerializersTests.DummyObjects
{
    public class DummySerializableObject : ISerializableObject
    {
        public DummySerializableObject()
        {
            Items = new List<DummyObject>();    
        }

        [JsonProperty("primary_property")]
        public IList<DummyObject> Items { get; set; }

        public string GetPrimaryPropertyName()
        {
            return "primary_property";
        }

        public Type GetPrimaryPropertyType()
        {
            return typeof (DummyObject);
        }
    }
}