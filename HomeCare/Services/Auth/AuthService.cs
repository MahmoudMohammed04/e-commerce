using HomeCare.Context;
using HomeCare.Models.UserSchema;
using HomeCare.Models.AuthSchema;
using HomeCare.Services.Result;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HomeCare.Services.Auth
{
    public class AuthService:IAuthService
    {
        //add role when register later 
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IJWTService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IGoogleAuth _googleAuth;

        public AuthService(
            AppDbContext context,
            UserManager<User> userManager,
            IJWTService jwtService,
            IEmailService emailService,
            IGoogleAuth googleAuth
            ) 
        {
            _context = context;
            _userManager = userManager;
            _jwtService = jwtService;
            _emailService = emailService;
            _googleAuth = googleAuth;
        }
        public async Task<ServiceResult<Token>> LoginAsync(AuthLoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return new ServiceResult<Token>("User not found",ErrorTypeEnum.NOT_FOUND);
            

            var result = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!result)
                return new ServiceResult<Token>("Wrong password",ErrorTypeEnum.BAD_REQUEST);
            
            Token token = new Token();
            token.AccessToken = await _jwtService.GenerateJwtToken(user);
            token.RefreshToken = _jwtService.GenerateRefreshToken();

         

            var refreshToken = new RefreshToken
            {
                Token = token.RefreshToken,
                UserId = user.Id,
                CreatedByIp = request.IPAddress,
                CreatedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7),
                DeviceName = request.DeviceName
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();


            return new ServiceResult<Token>(token);
        }

        public async Task<ServiceResult<Token>> RegisterAsync(AuthRegisterRequest request)
        {
            if(request.Password != request.ConfirmPassword)
                return new ServiceResult<Token>("Passwords do not match",ErrorTypeEnum.BAD_REQUEST);

            var user = _userManager.Users.Where(u => u.Email == request.Email).FirstOrDefault();

            if (user != null)
                 return new ServiceResult<Token>("User already exists",ErrorTypeEnum.CONFLICT);

            var newUser = new User
            {
                Id=Guid.NewGuid().ToString(),
                Email = request.Email,
                UserName = request.Email,
                EmailConfirmed = false,
                PhoneNumber = request.Phone,
                ConfirmationEmailCode = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(newUser, request.Password);

            if (result.Succeeded)
            {
                //await _userManager.AddToRoleAsync(newUser, "User");

                _emailService.SendEmailConfirmation(request.Email,newUser.Id,newUser.ConfirmationEmailCode);
                return await LoginAsync(new AuthLoginRequest
                {
                    Email = request.Email,
                    Password = request.Password,
                    IPAddress = request.IPAddress,
                    DeviceName = request.DeviceName
                    
                });
            }

            string error = string.Join(",", result.Errors.Select(x => x.Description));

            return new ServiceResult<Token>(error,ErrorTypeEnum.BAD_REQUEST);
        }

        public async Task<ServiceResult<Token>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

            if (refreshToken == null)
                return new ServiceResult<Token>("Invalid token", ErrorTypeEnum.BAD_REQUEST);

            if (refreshToken.Revoked != null)
            {
                await RevokeAllUserTokens(refreshToken.UserId);
                return new ServiceResult<Token>("Token reuse detected", ErrorTypeEnum.UNAUTHORIZED);
            }

            if (refreshToken.IsExpired)
                return new ServiceResult<Token>("Token expired", ErrorTypeEnum.BAD_REQUEST);

            var user = await _userManager.FindByIdAsync(refreshToken.UserId);

            if (user == null)
                return new ServiceResult<Token>("User not found", ErrorTypeEnum.NOT_FOUND);

            refreshToken.RevokedByIp = request.IPAddress;
            refreshToken.Revoked = DateTime.UtcNow;

            var newRefreshToken = _jwtService.GenerateRefreshToken();
            
            var newEntityRefreshToken = new RefreshToken
            {
                Token = newRefreshToken,
                UserId = user.Id,
                CreatedByIp = request.IPAddress,
                CreatedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7),
                DeviceName = request.DeviceName
                
            };
            
            refreshToken.ReplacedByToken = newEntityRefreshToken.Token;
            await _context.RefreshTokens.AddAsync(newEntityRefreshToken);
            await _context.SaveChangesAsync();


            var newToken = new Token
            {
                AccessToken = await _jwtService.GenerateJwtToken(user),
                RefreshToken = newRefreshToken
            };

            return new ServiceResult<Token>(newToken);

        }

        public async Task<ServiceResult<Token>> GoogleLoginAsync(string token)
        {
            var result = await _googleAuth.GoogleLoginRequest(token);
            Token? tokens = null;
            if(result.Success)
            {
                tokens = new Token
                {
                    AccessToken = await _jwtService.GenerateJwtToken(result.Data),
                    RefreshToken = _jwtService.GenerateRefreshToken()
                };

                return new ServiceResult<Token>(tokens);
            }

            return new ServiceResult<Token>(result.ErrorMessage,result.ErrorType);
        }

        private async Task RevokeAllUserTokens(string userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.IsActive)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.Revoked = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<ServiceResult<bool>> LogoutAsync(LogoutRequest request)
        {
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == request.Token);

            if (refreshToken == null)
                return new ServiceResult<bool>("Invalid token", ErrorTypeEnum.BAD_REQUEST);

            if (refreshToken.Revoked != null)
                return new ServiceResult<bool>("Token reuse detected", ErrorTypeEnum.UNAUTHORIZED);

            if (refreshToken.IsExpired)
                return new ServiceResult<bool>("Token expired", ErrorTypeEnum.BAD_REQUEST);

            refreshToken.RevokedByIp = request.IPAddress;
            refreshToken.Revoked = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return new ServiceResult<bool>(true);
        }

        public async Task<ServiceResult<bool>> ForgetPassword(string email,string code, string newPassword, string confirmPassword)
        {
            if(newPassword != confirmPassword)
                return new ServiceResult<bool>("Passwords do not match",ErrorTypeEnum.BAD_REQUEST);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return new ServiceResult<bool>("User not found", ErrorTypeEnum.NOT_FOUND);

            if(user.ResetPasswordCode != code || user.ResetPasswordCodeExpire < DateTime.UtcNow)
                return new ServiceResult<bool>("Invalid code", ErrorTypeEnum.BAD_REQUEST);

            user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, newPassword);
            user.ResetPasswordCode = null;
            user.ResetPasswordCodeExpire = null;
            user.SecurityStamp = Guid.NewGuid().ToString();

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return new ServiceResult<bool>(true);

            return new ServiceResult<bool>(
                result.Errors.FirstOrDefault()?.Description ?? "Error",
                ErrorTypeEnum.BAD_REQUEST
            );
        }

        

    }
}
