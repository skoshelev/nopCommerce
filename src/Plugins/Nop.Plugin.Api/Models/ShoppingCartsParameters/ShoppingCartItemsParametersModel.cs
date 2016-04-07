using System.Web.Http.ModelBinding;
using Nop.Plugin.Api.ModelBinders;

namespace Nop.Plugin.Api.Models.ShoppingCartsParameters
{
    [ModelBinder(typeof(ParametersModelBinder<ShoppingCartItemsParametersModel>))]
    public class ShoppingCartItemsParametersModel : BaseShoppingCartItemsParametersModel
    {
        public ShoppingCartItemsParametersModel()
        {
            CustomerId = 0;
        }
        
        public int CustomerId { get; set; }
    }
}