using System;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Models.ShoppingCartsParameters
{
    public class BaseShoppingCartItemsParametersModel
    {
        public BaseShoppingCartItemsParametersModel()
        {
            CreatedAtMin = null;
            CreatedAtMax = null;
            UpdatedAtMin = null;
            UpdatedAtMax = null;
            Limit = Configurations.DefaultLimit;
            Page = Configurations.DefaultPageValue;
            Fields = string.Empty;
        }

        public DateTime? CreatedAtMin { get; set; }
        public DateTime? CreatedAtMax { get; set; }
        public DateTime? UpdatedAtMin { get; set; }
        public DateTime? UpdatedAtMax { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }
        public string Fields { get; set; }
    }
}