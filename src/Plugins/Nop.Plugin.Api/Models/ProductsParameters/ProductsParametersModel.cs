using System.Collections.Generic;
using System.Web.Http.ModelBinding;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Models.ProductsParameters
{
    [ModelBinder(typeof(ParametersModelBinder<ProductsParametersModel>))]
    public class ProductsParametersModel : BaseProductsParametersModel
    {
        public ProductsParametersModel()
        {
            Ids = null;
            Limit = Configurations.DefaultLimit;
            Page = Configurations.DefaultPageValue;
            SinceId = Configurations.DefaultSinceId;
            Fields = string.Empty;
        }

        public List<int> Ids { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }
        public int SinceId { get; set; }
        public string Fields { get; set; }
    }
}