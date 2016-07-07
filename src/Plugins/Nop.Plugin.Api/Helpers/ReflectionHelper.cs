using System;
using System.Reflection;
using Nop.Plugin.Api.Attributes;

namespace Nop.Plugin.Api.Helpers
{
    public static class ReflectionHelper
    {
        public static bool HasProperty(string propertyName, Type type)
        {
            return type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null;
        }

        public static DtoAttribute GetDtoAttribute(Type type)
        {
            DtoAttribute dtoAttribute = type.GetCustomAttribute(typeof(DtoAttribute)) as DtoAttribute; ;

            return dtoAttribute;
        }
    }
}