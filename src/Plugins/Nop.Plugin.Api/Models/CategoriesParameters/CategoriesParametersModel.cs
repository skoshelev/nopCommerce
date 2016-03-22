using System.Web.Http.ModelBinding;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Models.CategoriesParameters
{
    [ModelBinder(typeof(ParametersModelBinder<CategoriesParametersModel>))]
    public class CategoriesParametersModel : BaseCategoriesParametersModel
    {
        public CategoriesParametersModel()
        {
            Ids = string.Empty;
            Limit = Configurations.DefaultLimit;
            Page = Configurations.DefaultPageValue;
            SinceId = 0;
            Fields = string.Empty;
        }

        public string Ids { get; set; }

        public int Limit { get; set; }

        public int Page { get; set; }

        public int SinceId { get; set; }

        public string Fields { get; set; }
    }
}