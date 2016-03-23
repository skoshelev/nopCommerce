using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Models.ProductsParameters
{
    public class BaseProductsParametersModel
    {
        public BaseProductsParametersModel()
        {
            CreatedAtMin = string.Empty;
            CreatedAtMax = string.Empty;
            UpdatedAtMin = string.Empty;
            UpdatedAtMax = string.Empty;
            PublishedStatus = Configurations.PublishedStatus;
            VendorName = string.Empty;
            CategoryId = Configurations.DefaultCategoryId;
        }

        public string CreatedAtMin { get; set; }
        public string CreatedAtMax { get; set; }
        public string UpdatedAtMin { get; set; }
        public string UpdatedAtMax { get; set; }
        public string PublishedStatus { get; set; }
        public string VendorName { get; set; }
        public int CategoryId { get; set; }
    }
}