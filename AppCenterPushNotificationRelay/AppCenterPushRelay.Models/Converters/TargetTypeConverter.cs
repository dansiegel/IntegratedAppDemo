using System;
using Newtonsoft.Json;

namespace AppCenterPushRelay.Models.Converters
{
    internal class TargetTypeConverter : JsonConverter
    {
        private const string AudiencesTarget = "audiences_target";
        private const string DevicesTarget = "devices_target";

        public override bool CanConvert(Type objectType) => objectType == typeof(TargetType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.Value.ToString())
            {
                case AudiencesTarget:
                    return TargetType.Audiences;
                case DevicesTarget:
                    return TargetType.Devices;
                default:
                    throw new Exception($"Unable to parse {reader.Value} to TargetType");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if(value is TargetType targetType)
            {
                switch(targetType)
                {
                    case TargetType.Audiences:
                        writer.WriteValue(AudiencesTarget);
                        break;
                    case TargetType.Devices:
                        writer.WriteValue(DevicesTarget);
                        break;
                }
            }
            else
            {
                throw new NotSupportedException($"Unable to convert type {value.GetType().FullName}");
            }
        }
    }
}
