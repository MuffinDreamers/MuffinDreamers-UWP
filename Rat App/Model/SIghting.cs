// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using Rat_App;
//
//    var data = Sighting.FromJson(jsonString);
//
namespace Rat_App
{
    using System;
    using System.Net;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public partial class Sighting
    {
        [JsonProperty("borough")]
        public string Borough { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("dateCreated")]
        public DateTime DateCreated { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("locationType")]
        public string LocationType { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("streetAddress")]
        public string StreetAddress { get; set; }

        [JsonProperty("zipcode")]
        public long Zipcode { get; set; }

        public override string ToString()
        {
            return $"Sighting ID: {Id}, Date Created: {DateCreated.ToString("MM-dd-yyyy")}";
        }
    }

    public partial class Sighting
    {
        public static Sighting[] FromJson(string json) => JsonConvert.DeserializeObject<Sighting[]>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Sighting[] self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
