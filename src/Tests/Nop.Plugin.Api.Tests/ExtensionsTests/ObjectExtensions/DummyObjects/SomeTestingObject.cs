using System;

namespace Nop.Plugin.Api.Tests.ExtensionsTests.ObjectExtensions.DummyObjects
{
    public class SomeTestingObject
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
        public DateTime? DateTimeNullableProperty { get; set; }
        public bool? BooleanNullableStatusProperty { get; set; }
    }
}