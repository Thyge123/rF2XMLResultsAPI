using Newtonsoft.Json;

namespace rF2XMLTestAPI.Model
{
    public class Driver
    {
        public int Id { get; set; }

        [JsonProperty("Name")]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [JsonConverter(typeof(LapConverter))]
        public List<Lap>? Lap { get; set; }

        public string? CarClass { get; set; }

        public string? CarType { get; set; }

        public string? VehName { get; set; }
    }
}
