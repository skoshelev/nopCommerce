using System.Web.Http.ModelBinding;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Models.CustomersParameters
{
    [ModelBinder(typeof(ParametersModelBinder<CustomersParametersModel>))]
    public class CustomersParametersModel
    {
        public CustomersParametersModel()
        {
            Limit = Configurations.DefaultLimit;
            Page = Configurations.DefaultPageValue;
            SinceId = 0;
            Fields = string.Empty;
            CreatedAtMax = string.Empty;
            CreatedAtMin = string.Empty;
        }

        public int Limit { get; set; }
        public int Page { get; set; }
        public int SinceId { get; set; }
        public string Fields { get; set; }
        public string CreatedAtMin { get; set; }
        public string CreatedAtMax { get; set; }
    }
}