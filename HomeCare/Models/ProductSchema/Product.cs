using HomeCare.Models.UserSchema;

namespace HomeCare.Models.ProductSchema
{
    public class Product:IModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public int Quantity { get; set; } = 0;
        public bool IsAvailable => Quantity > 0;

        public int NumberOfSold { get; set; } = 0;

        public double AverageRating { get; set; } = 0;
        public int RatingCount { get; set; } = 0;

        public string Image { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int BrandId { get; set; }
        public Brand Brand { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public List<ProductAttribute> ProductAttributes { get; set; } = new List<ProductAttribute>();
        public List<FacetIndexTable> FacetIndexTables { get; set; } = new List<FacetIndexTable>();
        public List<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public List<Review> Reviews { get; set; } = new List<Review>();

        public List<Cart> Carts { get; set; }= new List<Cart>();

        public List<OrderProduct> Orders { get; set; } = new List<OrderProduct>();
    }
}
