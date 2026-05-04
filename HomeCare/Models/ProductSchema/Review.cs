using HomeCare.Models.UserSchema;
using System.ComponentModel.DataAnnotations;

namespace HomeCare.Models.ProductSchema
{
    public class Review : IModel
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; } 
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
