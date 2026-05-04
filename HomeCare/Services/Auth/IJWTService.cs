using HomeCare.Models.UserSchema;
using HomeCare.Models.AuthSchema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace HomeCare.Services.Auth
{
    public interface IJWTService
    {
        public Task<string> GenerateJwtToken(User user);


        public string GenerateRefreshToken();
        

    }
}
