using Microsoft.AspNetCore.Mvc;

namespace HomeCare.Controllers.ViewsControllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Productv");
            
        }
    }
}
