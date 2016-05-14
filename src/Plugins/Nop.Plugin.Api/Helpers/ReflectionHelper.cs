using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Http;
using System.Web.Http.Results;
using Nop.Plugin.Api.ContractResolvers;

namespace Nop.Plugin.Api.Helpers
{
    public static class ReflectionHelper
    {
        public static bool HasProperty(string propertyName, Type type)
        {
            return type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null;
        }
    }
}