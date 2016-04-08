using System;
using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Models.ProductsParameters
{
    public class BaseProductsParametersModel
    {
        public BaseProductsParametersModel()
        {
            CreatedAtMin = null;
            CreatedAtMax = null;
            UpdatedAtMin = null;
            UpdatedAtMax = null;
            PublishedStatus = null;
            VendorName = null;
            CategoryId = Configurations.DefaultCategoryId;
        }

        public DateTime? CreatedAtMin { get; set; }
        public DateTime? CreatedAtMax { get; set; }
        public DateTime? UpdatedAtMin { get; set; }
        public DateTime? UpdatedAtMax { get; set; }
        public bool? PublishedStatus { get; set; }
        public string VendorName { get; set; }
        public int CategoryId { get; set; }
    }
}