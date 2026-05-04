using HomeCare.Models.ProductSchema;
using System.ComponentModel.DataAnnotations;

namespace HomeCare.Models.UserSchema
{
    public class Cart : IModel
    {
        
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int quantity { get; set; } = 0;
        public decimal UnitPrice { get; set; }
        public User User { get; set; }
        public Product Product { get; set; }
    }
}
