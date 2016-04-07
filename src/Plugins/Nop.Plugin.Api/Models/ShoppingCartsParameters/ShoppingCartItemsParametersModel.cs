using System;
using System.Web.Http.ModelBinding;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Models.ShoppingCartsParameters
{
    [ModelBinder(typeof(ParametersModelBinder<ShoppingCartItemsParametersModel>))]
    public class ShoppingCartItemsParametersModel
    {
        public ShoppingCartItemsParametersModel()
        {
            CreatedAtMin = null;
            CreatedAtMax = null;
            UpdatedAtMin = null;
            UpdatedAtMax = null;
            CustomerId = 0;
            Limit = Configurations.DefaultLimit;
            Page = Configurations.DefaultPageValue;
            Fields = string.Empty;
        }

        public DateTime? CreatedAtMin { get; set; }
        public DateTime? CreatedAtMax { get; set; }
        public DateTime? UpdatedAtMin { get; set; }
        public DateTime? UpdatedAtMax { get; set; }
        public int CustomerId { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }
        public string Fields { get; set; }
    }
}