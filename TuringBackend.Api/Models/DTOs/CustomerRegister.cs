using Newtonsoft.Json;

namespace TuringBackend.Models
{
    public class CustomerRegister
    {
        [JsonProperty("customer")]
        public Customer Customer { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }


        [JsonProperty("expires_in")] 
        public string ExpiresIn { get; set; }
    }
}