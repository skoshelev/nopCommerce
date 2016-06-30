using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nop.Plugin.Api.Helpers
{
    public interface IMappingHelper
    {
        void SetValues(Dictionary<string, object> jsonPropertiesValuePairsPassed, object objectToBeUpdated, Type objectToBeUpdatedType);
        void ConvertAndSetValueIfValid(object objectToBeUpdated, PropertyInfo objectProperty, object propertyValue);
    }
}