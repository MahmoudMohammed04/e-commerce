using HomeCare.Extentions;
using HomeCare.Services.Products;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeCare.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IReviewService _reviewService;
        public ProductController(IProductService productService , IReviewService reviewService)
        {
            _productService = productService;
            _reviewService = reviewService;
        }

        [AllowAnonymous]
        [HttpGet("Products")]
        public async Task<ActionResult<List<ProductQueryResponse>>> GetProduct([FromQuery]ProductQueryRequest request,[FromQuery]int page)
        {
            var result = await _productService.GetProducts(request, page);

            return result.Success ? Ok(result.Data) : result.ErrorToGenericActionResult();
        }

        [AllowAnonymous]
        [HttpGet("ProductDetails")]
        public async Task<ActionResult<ProductDetailsResponse>> GetProductDetails([FromQuery] int id)
        {
            var result = await _productService.GetProductDetails(id);

            return result.Success ? Ok(result.Data) : result.ErrorToGenericActionResult();
        }

        //admin
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromForm] AddProductRequest request)
        {
            var result = await _productService.AddProduct(request);

            return result.Success ? Ok(result.Data) : result.ErrorToActionResult();
        }

        //admin
        [HttpDelete("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct([FromQuery] int id)
        {
            var result = await _productService.DeleteProduct(id);

            return result.Success ? Ok(result.Data) : result.ErrorToActionResult();
        }

        [HttpPost("AddReview")]
        public async Task<IActionResult> AddReview([FromBody] ReviewRequest request)
        {
            var result = await _reviewService.AddReview(GetUserId(),request);

            return result.Success ? Ok(result.Data) : result.ErrorToActionResult();
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
