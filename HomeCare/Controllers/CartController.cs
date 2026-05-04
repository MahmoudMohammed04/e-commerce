using HomeCare.Extentions;
using HomeCare.Services.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeCare.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

       
        [HttpPost("addToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var userId = GetUserId();

            var result = await _cartService.AddToCart(userId, request);

            return result.Success
                ? Ok(result.Data)
                : result.ErrorToActionResult();
        }

      
        [HttpDelete("removeFromCart")]
        public async Task<IActionResult> RemoveFromCart([FromBody] RemoveFromCartRequest request)
        {
            var userId = GetUserId();

            var result = await _cartService.RemoveFromCart(userId, request);

            return result.Success
                ? Ok(result.Data)
                : result.ErrorToActionResult();
        }

        [HttpGet("showCart")]
        public async Task<ActionResult<List<ProductQueryResponse>>> ShowCart([FromQuery] int page = 1, [FromQuery] int pageSize = 15)
        {
            var userId = GetUserId();

            var result = await _cartService.ShowCart(userId, page, pageSize);

            return result.Success
                ? Ok(result.Data)
                : result.ErrorToGenericActionResult();
        }

      
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();

            var result = await _cartService.ClearCart(userId);

            return result.Success
                ? Ok(result.Data)
                : result.ErrorToActionResult();
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<string>> Checkout([FromBody] CheckOutRequest request)
        {
            var userId = GetUserId();

            var result = await _cartService.CheckOut(userId, request);


            return result.Success
                ? Ok(result.Data)
                : result.ErrorToGenericActionResult();
        }

      
        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }

 
   
}