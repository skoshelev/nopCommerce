using System;
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
            CreatedAtMax = null;
            CreatedAtMin = null;
        }

        /// <summary>
        /// Amount of results
        /// <br />
        /// (default: 50) (maximum: 250)
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Page to show
        /// <br />
        /// (default: 1)
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Restrict results to after the specified ID
        /// </summary>
        public int SinceId { get; set; }

        /// <summary>
        /// Comma-separated list of fields to include in the response
        /// </summary>
        public string Fields { get; set; }

        /// <summary>
        /// Show customers created after date 
        /// <br />
        /// (format: 2008-12-31 03:00)
        /// </summary>
        public DateTime? CreatedAtMin { get; set; }

        /// <summary>
        /// Show customers created before date 
        /// <br />
        /// (format: 2008-12-31 03:00)
        /// </summary>
        public DateTime? CreatedAtMax { get; set; }
    }
}