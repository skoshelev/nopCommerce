using System.Collections.Generic;

namespace Nop.Plugin.Api.DTOs.BaseDtoTypes
{
    public interface IStoreMappable
    {
        List<int> StoreIds { get; set; }
    }
}