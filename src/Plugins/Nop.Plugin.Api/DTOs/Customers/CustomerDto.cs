using System.Collections.Generic;
using Newtonsoft.Json;
using Nop.Plugin.Api.DTOs.ShoppingCarts;

namespace Nop.Plugin.Api.DTOs.Customers
{
    [JsonObject(Title = "customers")]
    public class CustomerDto : BaseCustomerDto
    {
        private ICollection<ShoppingCartItemDto> _shoppingCartItems;
        private ICollection<AddressDto> _addresses;
        
        #region Navigation properties

        /// <summary>
        /// Gets or sets shopping cart items
        /// </summary>
        [JsonProperty("shopping_cart_items")]
        public ICollection<ShoppingCartItemDto> ShoppingCartItems
        {
            get { return _shoppingCartItems; }
            set { _shoppingCartItems = value; }
        }

        /// <summary>
        /// Default billing address
        /// </summary>
        [JsonProperty("billing_address")]
        public AddressDto BillingAddress { get; set; }

        /// <summary>
        /// Default shipping address
        /// </summary>
        [JsonProperty("shipping_address")]
        public AddressDto ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets customer addresses
        /// </summary>
        [JsonProperty("addresses")]
        public ICollection<AddressDto> Addresses
        {
            get { return _addresses; }
            set { _addresses = value; }
        }
        #endregion
    }
}
