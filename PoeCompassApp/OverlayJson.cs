﻿namespace OverlayJson
{
    
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class OverlayData
    {
        [JsonProperty("clientLogFile")]
        public string clientLogFile { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("zone")]
        public Zone[] Zone { get; set; }
    }

    public partial class Zone
    {
        [JsonProperty("zoneName")]
        public string ZoneName { get; set; }

        [JsonProperty("zoneSeed")]
        public object[] ZoneSeed { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }
    }

    public partial class OverlayData
    {
        public static OverlayData[] FromJson(string json) => JsonConvert.DeserializeObject<OverlayData[]>(json, OverlayJson.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this OverlayData[] self) => JsonConvert.SerializeObject(self, OverlayJson.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
