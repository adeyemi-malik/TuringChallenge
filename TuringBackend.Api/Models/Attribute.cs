using Newtonsoft.Json;

namespace TuringBackend.Models
{
    public class Attribute
    {
        [JsonProperty("attribute_id")]
        public int AttributeId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}