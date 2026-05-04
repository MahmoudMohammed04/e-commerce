using HomeCare.Services.Result;

namespace HomeCare.Services.Auth
{
    public interface IAuthService
    {
        public Task<ServiceResult<Token>> LoginAsync(AuthLoginRequest request);

        public Task<ServiceResult<Token>> RegisterAsync(AuthRegisterRequest request);

        public Task<ServiceResult<Token>> RefreshTokenAsync(RefreshTokenRequest token);
        public Task<ServiceResult<Token>> GoogleLoginAsync(string token);

        public Task<ServiceResult<bool>> LogoutAsync(LogoutRequest request);

        public  Task<ServiceResult<bool>> ForgetPassword(string email,string code, string newPassword, string confirmPassword);
    }

    public class AuthLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string IPAddress { get; set; }
        public string DeviceName { get; set; }
    }

    public class AuthRegisterRequest
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; } 
        public string ConfirmPassword { get; set; }
        public string Phone { get; set; }
        public string IPAddress { get; set; }
        public string DeviceName { get; set; }
    }

    public class Token
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
        public string IPAddress { get; set; }
        public string DeviceName { get; set; }
    }

    public class LogoutRequest
    {
        public string Token { get; set; }
        public string IPAddress { get; set; }
    }

    
}
