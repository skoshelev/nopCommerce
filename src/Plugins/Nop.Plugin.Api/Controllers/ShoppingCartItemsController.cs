using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Api.ActionResults;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.DTOs.ShoppingCarts;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.Models.ShoppingCartsParameters;
using Nop.Plugin.Api.MVC;
using Nop.Plugin.Api.Serializers;
using Nop.Plugin.Api.Services;

namespace Nop.Plugin.Api.Controllers
{
    [BearerTokenAuthorize]
    public class ShoppingCartItemsController : ApiController
    {
        private readonly IShoppingCartItemApiService _shoppingCartItemApiService;
        private readonly IJsonFieldsSerializer _jsonFieldsSerializer;

        public ShoppingCartItemsController(IShoppingCartItemApiService shoppingCartItemApiService, IJsonFieldsSerializer jsonFieldsSerializer)
        {
            _shoppingCartItemApiService = shoppingCartItemApiService;
            _jsonFieldsSerializer = jsonFieldsSerializer;
        }

        [HttpGet]
        [ResponseType(typeof(ShoppingCartItemsRootObject))]
        public IHttpActionResult GetShoppingCartItems(ShoppingCartItemsParametersModel parameters)
        {
            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return BadRequest("Invalid request parameters");
            }

            if (parameters.Page <= 0)
            {
                return BadRequest("Invalid request parameters");
            }

            IList<ShoppingCartItem> shoppingCartItems = _shoppingCartItemApiService.GetShoppingCartItems(parameters.CustomerId,
                                                                                                         parameters.CreatedAtMin,
                                                                                                         parameters.CreatedAtMax, parameters.UpdatedAtMin,
                                                                                                         parameters.UpdatedAtMax, parameters.Limit,
                                                                                                         parameters.Page);

            List<ShoppingCartItemDto> shoppingCartItemsDtos = shoppingCartItems.Select(x => x.ToDto()).ToList();

            var shoppingCartsRootObject = new ShoppingCartItemsRootObject()
            {
                ShoppingCartItems = shoppingCartItemsDtos
            };

            var json = _jsonFieldsSerializer.Serialize(shoppingCartsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }
        
        [HttpGet]
        [ResponseType(typeof(ShoppingCartItemsRootObject))]
        public IHttpActionResult GetShoppingCartItemsByCustomerId(int customerId, ShoppingCartItemsForCustomerParametersModel parameters)
        {
            if (customerId <= Configurations.DefaultCustomerId)
            {
                return NotFound();
            }

            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return BadRequest("Invalid request parameters");
            }

            if (parameters.Page <= 0)
            {
                return BadRequest("Invalid request parameters");
            }

            IList<ShoppingCartItem> shoppingCartItems = _shoppingCartItemApiService.GetShoppingCartItems(customerId,
                                                                                                         parameters.CreatedAtMin,
                                                                                                         parameters.CreatedAtMax, parameters.UpdatedAtMin,
                                                                                                         parameters.UpdatedAtMax, parameters.Limit,
                                                                                                         parameters.Page);

            if (shoppingCartItems == null)
            {
                return NotFound();
            }

            List<ShoppingCartItemDto> shoppingCartItemsDtos = shoppingCartItems.Select(x => x.ToDto()).ToList();

            var shoppingCartsRootObject = new ShoppingCartItemsRootObject()
            {
                ShoppingCartItems = shoppingCartItemsDtos
            };

            var json = _jsonFieldsSerializer.Serialize(shoppingCartsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }
    }
}