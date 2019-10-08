using System;
using Newtonsoft.Json;

namespace TuringBackend.Models
{
    public class Orders
    {
        [JsonProperty("order_id")]
        public int OrderId { get; set; }

        [JsonProperty("total_amount")]
        public decimal TotalAmount { get; set; }

        [JsonProperty("created_on")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("shipped_on")]
        public DateTime? ShippedOn { get; set; }
        public int Status { get; set; }
        public string Comments { get; set; }
        public int? CustomerId { get; set; }
        public string AuthCode { get; set; }
        public string Reference { get; set; }
        public int? ShippingId { get; set; }
        public int? TaxId { get; set; }
    }
}