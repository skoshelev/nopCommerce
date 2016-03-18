using Nop.Plugin.Api.DTOs.Customers;

namespace Nop.Plugin.Api.Serializers
{
    public interface IJsonFieldsSerializer
    {
        string Serialize(ISerializableObject objectToSerialize, string fields);
    }
}
