using Newtonsoft.Json;

namespace Nop.Plugin.Api.Tests.SerializersTests.DummyObjects
{
    public class DummyObject
    {
        public string FirstProperty { get; set; }

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