using Microsoft.AspNetCore.Mvc;

namespace HomeCare.Controllers.ViewsControllers
{
    public class AuthvController : Controller
    {

  
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View("LoginView");
        }

        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View("RegisterView");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            ViewData["Email"] = email;
            ViewData["Token"] = token;
            return View();
        }
    }
}
