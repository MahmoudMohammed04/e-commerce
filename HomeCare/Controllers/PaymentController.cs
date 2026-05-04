using HomeCare.Extentions;
using HomeCare.Models.UserSchema;
using HomeCare.Services.PaymentService;
using HomeCare.Services.Products;
using HomeCare.Services.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Security.Claims;

namespace HomeCare.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IConfiguration _configuration;
        private readonly ICartService _cartService;
        public PaymentController(IPaymentService paymentService, IOrderService orderService, IConfiguration configuration, ICartService cartService)
        {
            _paymentService = paymentService;
            _orderService = orderService;
            _configuration = configuration;
            _cartService = cartService;
        }


        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        [HttpPost("create-payment-intent")]
        public async Task<ActionResult<string>> CreatePaymentIntent([FromBody] PaymentIntentRequest request)
        {
            var result = await _paymentService.CreatePaymentIntent(GetUserId(), request);

            return result.Success ? Ok(result.Data) : result.ErrorToGenericActionResult();
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> Handle()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _configuration["Stripe:WebhookSecret"]
                );

            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var result = await HandlePaymentSuccess(stripeEvent);

                if (!result.Success)
                    return result.ErrorToActionResult();
            }

            return Ok();
        }

        private async Task<ServiceResult<bool>> HandlePaymentSuccess(Event stripeEvent)
        {
            var intent = stripeEvent.Data.Object as PaymentIntent;

            var orderId = intent.Metadata["orderId"];
            var userId = intent.Metadata["userId"];

            var Order = await _orderService.GetOrder(int.Parse(orderId));

            if (!Order.Success)
                return new ServiceResult<bool>("Order not found" , ErrorTypeEnum.NOT_FOUND);

            if(Order.Data.Status == OrderStatusEnum.Paid)
                return new ServiceResult<bool>("Order is already paid" , ErrorTypeEnum.BAD_REQUEST);

            var result = await _orderService.UpdateOrderToPiad(int.Parse(orderId));

            if (result.Success)
            {
                var clearResult =  await _cartService.ClearCart(userId);
                return clearResult;
            }

            return result;

        }
    }
}
