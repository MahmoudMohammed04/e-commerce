using Google.Apis.Auth;
using HomeCare.Context;
using HomeCare.Models.UserSchema;
using HomeCare.Models.AuthSchema;
using HomeCare.Services.Result;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Services.Auth
{
    public class GoogleAuth : IGoogleAuth
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;
        

        public GoogleAuth(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
          
        }
        public async Task<ServiceResult<User>>  GoogleLoginRequest(string idToken)
        {
            GoogleJsonWebSignature.Payload? response = null;
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[] { _config["Google:ClientId"] }
                    });

                response = payload;
            }
            catch (InvalidJwtException)
            {
                Console.WriteLine("Invalid Google token");
                return new ServiceResult<User>("Invalid Google token", ErrorTypeEnum.UNAUTHORIZED);
                
            }

            var googleId = response.Subject;
            var email = response.Email;
            var name = response.Name;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);

            if(user is null)
            {
                user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user is null)
                {
                    user = new User
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = email,
                        NormalizedEmail = email.ToUpper(),
                        UserName = name,
                        NormalizedUserName = name.ToUpper(),
                        GoogleId = googleId,
                        EmailConfirmed = true
                    };

                    await _context.Users.AddAsync(user);
                }
                else
                {
                    user.GoogleId = googleId;
                }
                    await _context.SaveChangesAsync();
            }
            
            return new ServiceResult<User>(user);
        }
    }
}
