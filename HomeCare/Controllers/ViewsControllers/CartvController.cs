using Microsoft.AspNetCore.Mvc;

namespace HomeCare.Controllers.ViewsControllers
{
    public class CartvController : Controller
    {
        public IActionResult Cart()
        {
            return View("Cart");
        }
    }
}
