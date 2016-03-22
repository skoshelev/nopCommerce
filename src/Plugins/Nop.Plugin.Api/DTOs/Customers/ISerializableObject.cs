using System;

namespace Nop.Plugin.Api.DTOs.Customers
{
    public interface ISerializableObject
    {
        string GetPrimaryPropertyName();
        Type GetPrimaryPropertyType();
    }
}