using HomeCare.Models.UserSchema;
using System.ComponentModel.DataAnnotations;

namespace HomeCare.Models.AuthSchema
{
    public class RefreshToken:IModel
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime CreatedAt { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        
        public string? RevokedByIp { get; set; }
        public string? ReplacedByToken { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;
        public string DeviceName { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
