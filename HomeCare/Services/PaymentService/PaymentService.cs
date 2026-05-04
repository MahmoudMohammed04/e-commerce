using HomeCare.Services.Result;
using Stripe;



namespace HomeCare.Services.PaymentService
{
    public class PaymentService:IPaymentService
    {
        public PaymentService(IConfiguration config)
        {
           
            StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
        }

        public async Task<ServiceResult<string>> CreatePaymentIntent(string UserId,PaymentIntentRequest  request)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(request.Amount * 100),
                Currency = "usd",
                Metadata = new Dictionary<string, string> {
                    { "orderId", request.OrderId.ToString() },
                    { "userId", UserId }
                  
                }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);

            return new ServiceResult<string>(intent.ClientSecret);
        }
    }
}
