using Newtonsoft.Json;

namespace rF2XMLTestAPI.Model
{
    public class Lap
    {
        public int Id { get; set; }
        public int DriverId { get; set; }

        [JsonProperty("#text")]
        public string LapTime { get; set; }

        [JsonProperty("@s1")]
        public string Sector1 { get; set; }
        [JsonProperty("@s2")]
        public string Sector2 { get; set; }
        [JsonProperty("@s3")]
        public string Sector3 { get; set; }

        public int RaceResultsId { get; set; }

        public string CarClass { get; set; }

         public string CarType { get; set; }
       
        [JsonProperty(propertyName: "@fuel")]
        public string Fuel { get; set; }

        public string VehName { get; set; }

    }
}
