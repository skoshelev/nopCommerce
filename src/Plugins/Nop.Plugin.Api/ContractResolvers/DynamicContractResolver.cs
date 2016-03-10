using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nop.Plugin.Api.ContractResolvers
{
    public class DynamicContractResolver : DefaultContractResolver
    {
        private Dictionary<string, bool> _propertiesToSerialize = null;

        public DynamicContractResolver(Dictionary<string, bool> propertiesToSerialize)
        {
            _propertiesToSerialize = propertiesToSerialize;
        }
        
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jsonProperty = base.CreateProperty(member, memberSerialization);

            jsonProperty.ShouldSerialize = o => _propertiesToSerialize.ContainsKey(jsonProperty.PropertyName);

            return jsonProperty;
        }
    }
}