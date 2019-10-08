using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TuringBackend.Models
{
    public class ShoppingCart
    {
        [JsonProperty("item_id")]
        public int ItemId { get; set; }
        [JsonProperty("cart_id")]
        public string CartId { get; set; }

        [JsonProperty("product_id")]
        public int ProductId { get; set; }

        [JsonProperty("attributes")]
        public string Attributes { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonIgnore]
        public sbyte BuyNow { get; set; }
        [JsonIgnore]
        public DateTime AddedOn { get; set; }

        
    }
}