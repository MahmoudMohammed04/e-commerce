
using HomeCare.Models.AuthSchema;
using HomeCare.Models.ProductSchema;
using Microsoft.AspNetCore.Identity;

namespace HomeCare.Models.UserSchema
{
    public class User : IdentityUser<string>,IModel
    {
        public string? GoogleId { get; set; }
        public string? ConfirmationEmailCode { get; set; }

        public string? MarhcantName { get; set; }
        public string? ResetPasswordCode { get; set; }
        public DateTime? ResetPasswordCodeExpire { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public List<Cart> CartItems { get; set; } = new List<Cart>();

        public List<Order> Orders { get; set; } = new List<Order>();

        public List<Product> Products { get; set; } = new List<Product>();
    }
}
