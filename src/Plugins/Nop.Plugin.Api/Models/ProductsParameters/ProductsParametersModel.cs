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
            Ids = string.Empty;
            Limit = Configurations.DefaultLimit;
            Page = Configurations.DefaultPageValue;
            SinceId = Configurations.DefaultSinceId;
            Fields = string.Empty;
        }

        public string Ids { get; set; }
        public int Limit { get; set; }
        public int Page { get; set; }
        public int SinceId { get; set; }
        public string Fields { get; set; }
    }
}