using HomeCare.Context;
using HomeCare.Extentions;
using HomeCare.Services.Auth;
using HomeCare.Services.Result;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using System.Net;
using System.Security.Cryptography;

namespace HomeCare.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly AppDbContext _context;
        public AuthController(IAuthService authService , IEmailService emailService, AppDbContext context)
        {
            _authService = authService;
            _emailService = emailService;
            _context = context;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<Token>> Login([FromBody] AuthLoginRequest request)
        {
            IServiceResult<Token> result = await _authService.LoginAsync(request);

            if(result.Success)
                return Ok(result.Data);

            return result.ErrorToGenericActionResult();
        }

        [HttpPost("Register")]
        public async Task<ActionResult<Token>> Register([FromBody] AuthRegisterRequest request)
        {
            IServiceResult<Token> result = await _authService.RegisterAsync(request);

            if (result.Success)
                return Ok(result.Data);

            return result.ErrorToGenericActionResult();
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<Token>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            IServiceResult<Token> result = await _authService.RefreshTokenAsync(request);

            if (result.Success)
                return Ok(result.Data);

            return result.ErrorToGenericActionResult();
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request )
        {
            IServiceResult<bool> result = await _authService.LogoutAsync(request);

            if (result.Success)
                return Ok("Logout successfully");

            return result.ErrorToActionResult();
        }

        [HttpPost("google-login")]
        public async Task<ActionResult<Token>> GoogleLogin([FromQuery] string token)
        {
            IServiceResult<Token> result = await _authService.GoogleLoginAsync(token);

            if (result.Success)
                return Ok(result.Data);

            return result.ErrorToGenericActionResult();
        }

        [HttpPost("test-mail")]
        public IActionResult SendMail(string to, string subject, string body)
        {
             _emailService.SendEmail(to, subject, body);
            return Ok();
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string userId)
        {
            token = WebUtility.UrlDecode(token);
            userId = WebUtility.UrlDecode(userId);
            var user = await _context.Users.FindAsync(userId);

            bool isSuccess = user != null && user.ConfirmationEmailCode == token;

            if (isSuccess)
            {
                user.EmailConfirmed = true;
                await _context.SaveChangesAsync();
            }

            string html = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Email Confirmation</title>
    <link href='https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap' rel='stylesheet'>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{
            font-family: 'Inter', -apple-system, BlinkMacSystemFont, sans-serif;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            overflow: hidden;
            position: relative;
        }}
        .shape {{
            position: absolute;
            filter: blur(80px);
            opacity: 0.6;
            animation: float 20s infinite ease-in-out;
        }}
        .shape-1 {{ width: 400px; height: 400px; background: #ff6b6b; border-radius: 50%; top: -200px; left: -100px; }}
        .shape-2 {{ width: 300px; height: 300px; background: #4ecdc4; border-radius: 50%; bottom: -150px; right: -100px; animation-delay: -5s; }}
        .shape-3 {{ width: 250px; height: 250px; background: #ffe66d; border-radius: 50%; top: 50%; left: 50%; animation-delay: -10s; }}
        @keyframes float {{
            0%, 100% {{ transform: translate(0, 0) scale(1); }}
            33% {{ transform: translate(30px, -30px) scale(1.1); }}
            66% {{ transform: translate(-20px, 20px) scale(0.9); }}
        }}
        .container {{ position: relative; z-index: 10; text-align: center; padding: 40px; }}
        .card {{
            background: rgba(255, 255, 255, 0.95);
            backdrop-filter: blur(20px);
            border-radius: 24px;
            padding: 60px 50px;
            box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
            max-width: 480px;
            width: 90vw;
            animation: slideUp 0.6s ease-out;
        }}
        @keyframes slideUp {{ from {{ opacity: 0; transform: translateY(30px); }} to {{ opacity: 1; transform: translateY(0); }} }}
        .icon-wrapper {{
            width: 100px; height: 100px; margin: 0 auto 30px; border-radius: 50%;
            display: flex; align-items: center; justify-content: center; font-size: 48px;
            animation: scaleIn 0.5s ease-out 0.3s both;
        }}
        .success .icon-wrapper {{
            background: linear-gradient(135deg, #10b981 0%, #059669 100%);
            box-shadow: 0 10px 30px -5px rgba(16, 185, 129, 0.4);
            color: white;
        }}
        .error .icon-wrapper {{
            background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%);
            box-shadow: 0 10px 30px -5px rgba(239, 68, 68, 0.4);
            color: white;
        }}
        @keyframes scaleIn {{ from {{ transform: scale(0); }} to {{ transform: scale(1); }} }}
        h1 {{
            font-size: 32px; font-weight: 700; margin-bottom: 16px; letter-spacing: -0.025em;
            {(isSuccess ?
                        "background: linear-gradient(135deg, #059669 0%, #10b981 100%); -webkit-background-clip: text; -webkit-text-fill-color: transparent;" :
                        "background: linear-gradient(135deg, #dc2626 0%, #ef4444 100%); -webkit-background-clip: text; -webkit-text-fill-color: transparent;")}
        }}
        p {{ font-size: 18px; color: #6b7280; line-height: 1.6; margin-bottom: 32px; }}
        .btn {{
            display: inline-flex; align-items: center; gap: 8px; padding: 16px 32px;
            border-radius: 12px; font-size: 16px; font-weight: 600; text-decoration: none;
            transition: all 0.3s ease; border: none; cursor: pointer;
        }}
        .btn-primary {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white;
            box-shadow: 0 4px 15px -2px rgba(102, 126, 234, 0.4);
        }}
        .btn-primary:hover {{ transform: translateY(-2px); box-shadow: 0 8px 25px -5px rgba(102, 126, 234, 0.5); }}
        .btn-secondary {{ background: #f3f4f6; color: #4b5563; margin-left: 12px; }}
        .btn-secondary:hover {{ background: #e5e7eb; transform: translateY(-2px); }}
        .dots {{ display: flex; justify-content: center; gap: 8px; margin-top: 40px; }}
        .dot {{ width: 8px; height: 8px; border-radius: 50%; background: #d1d5db; animation: pulse 1.4s infinite ease-in-out both; }}
        .dot:nth-child(1) {{ animation-delay: -0.32s; }}
        .dot:nth-child(2) {{ animation-delay: -0.16s; }}
        @keyframes pulse {{ 0%, 80%, 100% {{ transform: scale(0); opacity: 0.5; }} 40% {{ transform: scale(1); opacity: 1; }} }}
        .confetti {{
            position: fixed; width: 10px; height: 10px; top: -10px;
            animation: confetti-fall 3s linear forwards;
        }}
        @keyframes confetti-fall {{ to {{ transform: translateY(100vh) rotate(360deg); opacity: 0; }} }}
        @media (max-width: 640px) {{
            .card {{ padding: 40px 30px; }}
            h1 {{ font-size: 24px; }}
            p {{ font-size: 16px; }}
            .btn {{ width: 100%; justify-content: center; }}
            .btn-secondary {{ margin-left: 0; margin-top: 12px; }}
        }}
    </style>
</head>
<body>
    <div class='shape shape-1'></div>
    <div class='shape shape-2'></div>
    <div class='shape shape-3'></div>
    <div class='container'>
        <div class='card {(isSuccess ? "success" : "error")}'>
            <div class='icon-wrapper'>{(isSuccess ? "✓" : "✕")}</div>
            <h1>{(isSuccess ? "Email Confirmed!" : "Link Expired")}</h1>
            <p>{(isSuccess ?
                        "Your email has been successfully verified. You're all set to access your account and explore all features." :
                        "This confirmation link is invalid or has expired. Please request a new verification email to continue.")}</p>
           
            <div class='dots'><div class='dot'></div><div class='dot'></div><div class='dot'></div></div>
        </div>
    </div>
    {(isSuccess ? @"
    <script>
        const colors = ['#667eea', '#764ba2', '#10b981', '#f59e0b', '#ef4444'];
        for (let i = 0; i < 50; i++) {{
            setTimeout(() => {{
                const confetti = document.createElement('div');
                confetti.className = 'confetti';
                confetti.style.left = Math.random() * 100 + 'vw';
                confetti.style.backgroundColor = colors[Math.floor(Math.random() * colors.length)];
                confetti.style.animationDuration = (Math.random() * 2 + 2) + 's';
                confetti.style.borderRadius = Math.random() > 0.5 ? '50%' : '0';
                document.body.appendChild(confetti);
                setTimeout(() => confetti.remove(), 4000);
            }}, i * 50);
        }}
    </script>" : "")}
</body>
</html>";

            return Content(html, "text/html");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(string email,string token,string newPassword,string confirmPassword)
        {
            var result = await _authService.ForgetPassword(email,token,newPassword,confirmPassword);

            if (result.Success)
                return Ok("Reset password successfully");

            return result.ErrorToActionResult();
        }

    

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
                return Ok("If this email exists, a reset code was sent");


            var token = RandomNumberGenerator.GetInt32(0, 1000000).ToString("D6");
            user.ResetPasswordCode =  token;
            user.ResetPasswordCodeExpire = DateTime.UtcNow.AddMinutes(3);
            await _context.SaveChangesAsync();

            _emailService.SendPasswordReset(email,user.ResetPasswordCode);
            return Ok("Email sent successfully");
        }

    }
}
