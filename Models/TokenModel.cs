using Newtonsoft.Json;

namespace API_Assessment.Models
{
    public class TokenModel
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }
        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }
        [JsonProperty(PropertyName = "expires_in")]
        public string ExpiresIn { get; set; }
        public string DisplayName { get; set; }
        [JsonProperty(PropertyName = ".issued")]
        public string Issued { get; set; }
        [JsonProperty(PropertyName = ".expires")]
        public string Expires { get; set; }
    }
}
