using Newtonsoft.Json;

namespace TuringBackend.Models
{
    public class OrderDetail
    {
        public int ItemId { get; set; }
        [JsonProperty("order_id")]
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string Attributes { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }
}