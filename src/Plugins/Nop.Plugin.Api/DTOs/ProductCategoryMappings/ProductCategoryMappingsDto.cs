﻿using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.ProductCategoryMappings
{
    [JsonObject(Title = "product_category_mappings")]
    public class ProductCategoryMappingDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        [JsonProperty("product_id")]
        public int? ProductId { get; set; }

        /// <summary>
        /// Gets or sets the category identifier
        /// </summary>
        [JsonProperty("category_id")]
        public int? CategoryId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the product is featured
        /// </summary>
        [JsonProperty("is_futured_product")]
        public bool? IsFeaturedProduct { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        [JsonProperty("display_order")]
        public int? DisplayOrder { get; set; }
    }
}