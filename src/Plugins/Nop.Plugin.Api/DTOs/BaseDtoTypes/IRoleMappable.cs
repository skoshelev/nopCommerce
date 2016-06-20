using System.Collections.Generic;

namespace Nop.Plugin.Api.DTOs.BaseDtoTypes
{
    public interface IRoleMappable
    {
        List<int> RoleIds { get; set; }
    }
}