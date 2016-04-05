using Nop.Plugin.Api.MVC;

namespace Nop.Plugin.Api.Models.ProductCategoryMappingsParameters
{
    public class BaseCategoryMappingsParametersModel
    {
        public BaseCategoryMappingsParametersModel()
        {
            ProductId = Configurations.DefaultProductId;
            CategoryId = Configurations.DefaultCategoryId;
        }

        public int ProductId { get; set; }
        public int CategoryId { get; set; } 
    }
}