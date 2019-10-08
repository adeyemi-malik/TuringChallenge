using Newtonsoft.Json;

namespace TuringBackend.Models
{
    public class CartWithProduct
    {
        [JsonProperty("item_id")] 
        public int ItemId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("attributes")]
        public string Attributes { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("product_id")] 
        public int ProductId { get; set; }

        [JsonProperty("subtotal")]
        public string SubTotal { get; set; }
    }
}