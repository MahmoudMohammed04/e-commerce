using System.Text.Json.Serialization;

namespace HomeCare.Models.ProductSchema
{
    public class Brand:IModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int CategoryID { get; set; }

        [JsonIgnore]
        public List<Product> Products { get; set; } = new List<Product>();

        [JsonIgnore]
        public Category Category { get; set; }
    }
}
