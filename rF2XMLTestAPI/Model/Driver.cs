using Newtonsoft.Json;

namespace rF2XMLTestAPI.Model
{
    public class Driver
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [JsonConverter(typeof(LapConverter))]
        public List<Lap>? Lap { get; set; }

    }
}
