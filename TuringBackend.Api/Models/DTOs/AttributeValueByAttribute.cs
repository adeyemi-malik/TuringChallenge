using Newtonsoft.Json;

namespace TuringBackend.Models
{
    public class AttributeValueByAttribute
    {
        [JsonProperty("attribute_value_id")] 
        public int AttributeValueId { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}