using HomeCare.Extentions;
using HomeCare.Services.Products;
using HomeCare.Services.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeCare.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

     
        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var userId = GetUserId();

            var result = await _orderService.CreateOrder(userId, request);

            return result.Success
                ? Ok(result.Data)
                : result.ErrorToActionResult();
        }

        [HttpGet("orders")]
        public async Task<ActionResult<List<OrderQueryResponse>>> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 15)
        {
            var userId = GetUserId();

            var result = await _orderService.GetOrders(userId, page, pageSize);

            return result.Success
                ? Ok(result.Data)
                : result.ErrorToGenericActionResult();
        }

       
        [HttpGet("{orderId:int}")]
        public async Task<ActionResult<OrderDetailsResponse>> GetOrder(int orderId)
        {
            var userId = GetUserId();

            var result = await _orderService.GetOrderDetails(userId, orderId);

            return result.Success
                ? Ok(result.Data)
                : result.ErrorToGenericActionResult();
        }

      
        [HttpDelete("{orderId:int}")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var userId = GetUserId();

            var result = await _orderService.CancelOrder(userId, orderId);

            return result.Success
                ? Ok(result.Data)
                : result.ErrorToActionResult();
        }

      
        [HttpPatch("{orderId:int}/paid")]
        public async Task<IActionResult> MarkAsPaid(int orderId)
        {
            var result = await _orderService.UpdateOrderToPiad(orderId);

            return result.Success
                ? Ok(result.Data)
                : result.ErrorToActionResult();
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}