using System.Collections.Generic;
using Newtonsoft.Json;

namespace AppCenterPushRelay.Models
{
    public class NotificationRequest
    {
        [JsonProperty("notification_targets")]
        public IDictionary<string, NotificationTarget> Targets { get; set; }

        [JsonProperty("notification_content")]
        public NotificationContent Content { get; set; }
    }
}
