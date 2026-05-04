using HomeCare.Services.Result;

namespace HomeCare.Services.PaymentService
{
    public interface IPaymentService
    {
        Task<ServiceResult<string>> CreatePaymentIntent(string UserId,PaymentIntentRequest request);
    }

    public class PaymentIntentRequest
    {
        public PaymentRequest PaymentRequest { get; set; }

        public decimal Amount { get; set; }
        public int OrderId { get; set; }

        
    }

    public class PaymentRequest
    {
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
        public string CVV { get; set; }
    }
}
