using Microsoft.AspNetCore.Mvc;

namespace HomeCare.Controllers.ViewsControllers
{
    public class ProfilevController : Controller
    {
        public IActionResult Profile()
        {
            return View("profile");
        } 
    }
}
