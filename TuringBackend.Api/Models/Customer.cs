using Newtonsoft.Json;

namespace TuringBackend.Models
{
    public class Customer
    {
        [JsonProperty("customer_id")]
        public int CustomerId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        [JsonProperty("credit_card")]
        public string CreditCard { get; set; }

        [JsonProperty("address_1")]
        public string Address1 { get; set; }

        [JsonProperty("address_2")]
        public string Address2 { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("shipping_region_id")]
        public int ShippingRegionId { get; set; }

        [JsonProperty("day_phone")]
        public string DayPhone { get; set; }

        [JsonProperty("eve_phone")]
        public string EvePhone { get; set; }

        [JsonProperty("mob_phone")]
        public string MobPhone { get; set; }
    }
}