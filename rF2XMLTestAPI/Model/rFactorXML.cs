using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace rF2XMLTestAPI.Model
{
    
    public class LapConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(List<Lap>));
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Array)
            {
                return token.ToObject<List<Lap>>();
            }
            else if (token.Type == JTokenType.Object)
            {
                return new List<Lap> { token.ToObject<Lap>() };
            }
            else
            {
                throw new JsonSerializationException("Unexpected token type for Lap");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class Practice1
    {
        public List<Driver> Driver { get; set; }
    }

    public class rFactorXML
    {
        public RaceResults RaceResults { get; set; }
    }

    public class Root
    {
        public rFactorXML rFactorXML { get; set; }
    }
}
