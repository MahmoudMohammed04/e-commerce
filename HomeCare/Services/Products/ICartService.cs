using HomeCare.Services.PaymentService;
using HomeCare.Services.Result;
using Stripe;
using System.ComponentModel.DataAnnotations;

namespace HomeCare.Services.Products
{
    public interface ICartService
    {
        Task<ServiceResult<bool>> AddToCart(string userId,AddToCartRequest request);

        Task<ServiceResult<bool>> RemoveFromCart(string userId, RemoveFromCartRequest request);

        Task<ServiceResult<List<ProductQueryResponse>>> ShowCart(string userId, int page = 1, int pageSize = 15);

        Task<ServiceResult<bool>> ClearCart(string userId);
        
        Task<ServiceResult<string>> CheckOut(string userId, CheckOutRequest request);
    }

    public class AddToCartRequest
    {
        public int productId { get; set; }
        public int quantity { get; set; }
     
    }

    public class RemoveFromCartRequest
    {
        public int ProductId { get; set; }
        
      
    }

    public class CheckOutRequest : IValidatableObject
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string StreetAddress { get; set; }

        public int cityId { get; set; }
        public int countryId { get; set; }
        public string? state { get; set; }
        public string? PostalCode { get; set; }

        public PaymentRequest PaymentRequest { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(PaymentRequest.ExpirationMonth < 1 || PaymentRequest.ExpirationMonth > 12)
                yield return new ValidationResult("Invalid expiration month");

            if (PaymentRequest.ExpirationYear < DateTime.Now.Year || PaymentRequest.ExpirationYear > 3000)
                yield return new ValidationResult("Invalid expiration year");

            if(PaymentRequest.CVV.Length != 3)
                yield return new ValidationResult("Invalid CVV");

            if(PaymentRequest.CardNumber.Length != 16)
                yield return new ValidationResult("Invalid card number");

            if(PaymentRequest.CardHolderName.Length < 3)
                yield return new ValidationResult("Invalid card holder name");

            if(PhoneNumber.Length != 11)
                yield return new ValidationResult("Invalid phone number");

            if(PostalCode!.Length != 5)
                yield return new ValidationResult("Invalid postal code");

            if(FullName.Length < 3)
                yield return new ValidationResult("Invalid full name");

            if(Email.Length < 3)
                yield return new ValidationResult("Invalid email");


        }

    }
}
