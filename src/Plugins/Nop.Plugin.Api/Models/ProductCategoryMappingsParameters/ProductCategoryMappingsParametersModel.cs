using System.Web.Http.ModelBinding;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Models.ProductCategoryMappingsParameters
{
    [ModelBinder(typeof(ParametersModelBinder<ProductCategoryMappingsParametersModel>))]
    public class ProductCategoryMappingsParametersModel : BaseCategoryMappingsParametersModel
    {
        public ProductCategoryMappingsParametersModel()
        {
            SinceId = Configurations.DefaultSinceId;
            Page = Configurations.DefaultPageValue;
            Limit = Configurations.DefaultLimit;
            Fields = string.Empty;
        }

        public int SinceId { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
        public string Fields { get; set; }
    }
}