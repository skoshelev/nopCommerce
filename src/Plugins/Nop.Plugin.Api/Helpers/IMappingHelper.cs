using System;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Helpers
{
    public interface IMappingHelper
    {
        void SetValues(Dictionary<string, object> propertiesAsDictionary, object objectToBeUpdated, Type propertyType);
    }
}