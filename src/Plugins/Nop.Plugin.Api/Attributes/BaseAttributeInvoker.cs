using System;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Attributes
{
    public abstract class BaseAttributeInvoker : Attribute
    {
        public abstract void Invoke(object instance);
        public  abstract Dictionary<string, string> GetErrors();
    }
}