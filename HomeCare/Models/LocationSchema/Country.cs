using System.Text.Json.Serialization;

namespace HomeCare.Models.LocationSchema
{
    public class Country : IModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public List<City> Cities { get; set; } = new List<City>();
    }
}
