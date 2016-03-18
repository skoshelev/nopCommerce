using System;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Validators
{
    public interface IFieldsValidator
    {
        Dictionary<string, bool> GetValidFields(string fields, Type type);
    }
}