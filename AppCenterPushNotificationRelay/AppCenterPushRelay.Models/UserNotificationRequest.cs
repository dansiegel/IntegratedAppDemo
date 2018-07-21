using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AppCenterPushRelay.Models
{
    public class UserNotificationRequest
    {
        [JsonProperty("users")]
        public IList<Guid> Users { get; set; }

        [JsonProperty("notification_content")]
        public NotificationContent Content { get; set; }
    }
}
