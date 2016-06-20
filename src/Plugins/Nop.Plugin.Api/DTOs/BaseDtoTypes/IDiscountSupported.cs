using System.Collections.Generic;

namespace Nop.Plugin.Api.DTOs.BaseDtoTypes
{
    public interface IDiscountsSupported
    {
        List<int> DiscountIds { get; set; }
    }
}