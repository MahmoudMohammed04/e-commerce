using Microsoft.AspNetCore.Mvc;

namespace HomeCare.Controllers.ViewsControllers
{
    public class ProductvController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            ViewData["ProductId"] = id;
            return View("DetailsView");
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View("AddProductView");
        }

        [HttpGet]
        public IActionResult Cart()
        {
          
            return View("Cart");
        }
    }
}
