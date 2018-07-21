using Newtonsoft.Json;

namespace AppCenterPushRelay.Models
{
    public class ErrorDetails
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
