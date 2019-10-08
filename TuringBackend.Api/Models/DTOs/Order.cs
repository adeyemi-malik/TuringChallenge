using System;
using Newtonsoft.Json;

namespace TuringBackend.Models
{
    public class Order
    {
        [JsonProperty("order_id")]
        public int OrderId { get; set; }

        [JsonProperty("total_amount")]
        public decimal TotalAmount { get; set; }

        [JsonProperty("created_on")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("shipped_on")]
        public DateTime? ShippedOn { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}