using System.Text.Json.Serialization;

namespace HomeCare.Models.ProductSchema
{
    public class Category:IModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public List<Product> Products { get; set; } = new List<Product>();

        [JsonIgnore]
        public List<CategoryAttribute> CategoryAttributes { get; set; } = new List<CategoryAttribute>();

        [JsonIgnore]
        public List<Brand> Brands { get; set; } = new List<Brand>(); 
    }
}
