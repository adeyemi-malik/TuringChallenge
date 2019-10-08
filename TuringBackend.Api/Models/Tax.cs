using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TuringBackend.Models
{
    public class Tax
    {
        [JsonProperty("tax_id")]
        public int TaxId { get; set; }

        [JsonProperty("tax_type")]
        public string TaxType { get; set; }

        [JsonProperty("tax_percentage")]
        public decimal TaxPercentage { get; set; }
    }
}