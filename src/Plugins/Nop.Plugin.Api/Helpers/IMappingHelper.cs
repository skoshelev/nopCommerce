using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nop.Plugin.Api.Helpers
{
    public interface IMappingHelper
    {
        void SetValues(Dictionary<string, object> jsonPropertiesValuePairsPassed, object objectToBeUpdated, Type propertyType);
        Dictionary<string, object> GetChangedProperties();
        void ConverAndSetValueIfValid(object objectToBeUpdated, PropertyInfo objectProperty, object propertyValue);
    }
}