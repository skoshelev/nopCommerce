using System.Web.Http.ModelBinding;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Models.CustomersParameters
{
    [ModelBinder(typeof(ParametersModelBinder<CustomersSearchParametersModel>))]
    public class CustomersSearchParametersModel
    {
        public CustomersSearchParametersModel()
        {
            Order = "Id";
            Query = string.Empty;
            Page = Configurations.DefaultPageValue;
            Limit = Configurations.DefaultLimit;
            Fields = string.Empty;
        }

        /// <summary>
        /// Field and direction to order results by
        /// <br />
        /// (default: id DESC)
        /// </summary>
        public string Order { get; set; }

        /// <summary>
        /// Text to search customers
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Page to show
        /// <br />
        /// (default: 1)
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Amount of results
        /// <br />
        /// (default: 50) (maximum: 250)
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Comma-separated list of fields to include in the response
        /// </summary>
        public string Fields { get; set; }
    }
}