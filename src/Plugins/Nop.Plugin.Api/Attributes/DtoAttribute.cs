using System;

namespace Nop.Plugin.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DtoAttribute : Attribute
    {
        public Type ValidatorType { get; set; }
        public string RootProperty { get; set; }
    }
}