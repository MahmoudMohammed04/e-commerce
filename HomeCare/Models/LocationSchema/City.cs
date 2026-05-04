using System.Text.Json.Serialization;

namespace HomeCare.Models.LocationSchema
{
    public class City:IModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }

        [JsonIgnore]
        public Country Country { get; set; }
    }
}
