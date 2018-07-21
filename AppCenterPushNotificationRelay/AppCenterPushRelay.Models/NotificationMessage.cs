using Newtonsoft.Json;

namespace AppCenterPushRelay.Models
{
    public class NotificationMessage
    {
        [JsonProperty("notification_target")]
        public NotificationTarget Target { get; set; }

        [JsonProperty("notification_content")]
        public NotificationContent Content { get; set; }
    }
}
