using Newtonsoft.Json;

namespace TuringBackend.Models
{
    public class CustomerReview
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("review")]
        public string Review { get; set; }

        [JsonProperty("rating")]
        public int Rating { get; set; }

        [JsonProperty("created_on")] 
        public string CreatedOn { get; set; }
    }
}