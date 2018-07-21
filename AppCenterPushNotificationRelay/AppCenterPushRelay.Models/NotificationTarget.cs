using System;
using System.Collections.Generic;
using AppCenterPushRelay.Models.Converters;
using Newtonsoft.Json;

namespace AppCenterPushRelay.Models
{
    public class NotificationTarget
    {
        [JsonProperty("type", ItemConverterType = typeof(TargetTypeConverter))]
        public TargetType Type { get; set; }

        [JsonProperty("audiences")]
        public IEnumerable<string> Audiences { get; set; }

        [JsonProperty("devices")]
        public IEnumerable<Guid> Devices { get; set; }
    }
}
