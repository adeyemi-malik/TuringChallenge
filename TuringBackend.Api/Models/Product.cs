using Newtonsoft.Json;

namespace TuringBackend.Models
{
    public class Product
    {
        [JsonProperty("product_id")]
        public int ProductId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("discounted_price")]
        public decimal DiscountedPrice { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("image2")]
        public string Image2 { get; set; }

        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }

        [JsonProperty("display")]
        public short Display { get; set; }
    }
}