using Newtonsoft.Json;

namespace rF2XMLTestAPI.Model
{
    public class Lap
    {
        public int Id { get; set; }
        public int DriverId { get; set; }

        [JsonProperty("#text")]
        public string LapTime { get; set; }

        public int RaceResultsId { get; set; }

    }
}
