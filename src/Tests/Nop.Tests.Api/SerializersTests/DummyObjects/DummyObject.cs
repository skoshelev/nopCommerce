using Newtonsoft.Json;

namespace Nop.Plugin.Api.Tests.SerializersTests.DummyObjects
{
    public class DummyObject
    {
        [JsonProperty("first_property")]
        public string FirstProperty { get; set; }

        [JsonProperty("second_property")]
        public string SecondProperty { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is DummyObject)
            {
                var that = obj as DummyObject;

                return that.FirstProperty == FirstProperty && that.SecondProperty == SecondProperty;
            }

            return false;
        }
    }
}