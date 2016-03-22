using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Models.CategoriesParameters
{
    public class BaseCategoriesParametersModel
    {
        public BaseCategoriesParametersModel()
        {
            ProductId = 0;
            CreatedAtMin = string.Empty;
            CreatedAtMax = string.Empty;
            UpdatedAtMin = string.Empty;
            UpdatedAtMax = string.Empty;
            PublishedStatus = Configurations.PublishedStatus;
        }

        public string CreatedAtMin { get; set; }

        public string CreatedAtMax { get; set; }

        public string UpdatedAtMin { get; set; }

        public string UpdatedAtMax { get; set; }

        public string PublishedStatus { get; set; }

        public int ProductId { get; set; }
    }
}