using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TuringBackend.Models
{
    public class Shipping
    {
        [JsonProperty("shipping_id")]
        public int ShippingId { get; set; }

        [JsonProperty("shipping_type")]
        public string ShippingType { get; set; }

        [JsonProperty("shipping_cost")]
        public decimal ShippingCost { get; set; }

        [JsonProperty("shipping_region_id")]
        public int ShippingRegionId { get; set; }
    }
}