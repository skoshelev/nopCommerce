using Newtonsoft.Json;

namespace Nop.Plugin.Api.DTOs.Images
{
    public class ImageDto
    {
        [JsonProperty("src")]
        public string Src { get; set; }

        [JsonProperty("attachment")]
        public string Attachment { get; set; }
    }
}