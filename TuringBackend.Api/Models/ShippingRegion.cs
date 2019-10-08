using Newtonsoft.Json;

namespace TuringBackend.Models
{
    public class ShippingRegion
    {
        [JsonProperty("shipping_region_id")]
        public int ShippingRegionId { get; set; }
        [JsonProperty("shipping_region")]
        public string ShippingRegion1 { get; set; }
    }
}