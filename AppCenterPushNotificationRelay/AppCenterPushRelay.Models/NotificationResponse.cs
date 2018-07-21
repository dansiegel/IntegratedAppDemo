using Newtonsoft.Json;

namespace AppCenterPushRelay.Models
{
    public class NotificationResponse
    {
        [JsonProperty("notification_id")]
        public string NotificationId { get; set; }

        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
        public ErrorDetails Error { get; set; }
    }
}
