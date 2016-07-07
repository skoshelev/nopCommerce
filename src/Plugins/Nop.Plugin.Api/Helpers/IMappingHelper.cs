using System;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Helpers
{
    public interface IMappingHelper
    {
        void SetValues(Dictionary<string, object> jsonPropertiesValuePairsPassed, object objectToBeUpdated, Type objectToBeUpdatedType);
    }
}