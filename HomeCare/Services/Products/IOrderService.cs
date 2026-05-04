using HomeCare.Models.UserSchema;
using HomeCare.Services.Result;

namespace HomeCare.Services.Products
{
    public interface IOrderService
    {
         Task<ServiceResult<Order>> CreateOrder(string userId, CreateOrderRequest request);
         Task<ServiceResult<List<OrderQueryResponse>>> GetOrders(string userId, int page = 1, int pageSize = 15);

         Task<ServiceResult<List<ProductQueryResponse>>> GetOrderProducts(string userId, int orderId);

         Task<ServiceResult<Order>> GetOrder(int orderId);

         Task<ServiceResult<bool>> UpdateOrderToPiad(int orderId);

         Task<ServiceResult<bool>> CancelOrder(string userId, int orderId);

        Task<ServiceResult<OrderDetailsResponse>> GetOrderDetails(string userId, int orderId);
    }

    public class CreateOrderRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string StreetAddress { get; set; }

        public int cityId { get; set; }
        public int countryId { get; set; }
        public string? state { get; set; }
        public string? PostalCode { get; set; }
        public List<OrderProductDetials> Products { get; set; }
    }

    public class OrderProductDetials
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderQueryResponse
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public OrderStatusEnum Status { get; set; }
        public DateTime CreatedDate { get; set; }
       
    }

    public class OrderDetailsResponse
    {
        public int Id { get; set; }
        public string StreetAddress { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public string? state { get; set; }
        public string? PostalCode { get; set; }

        public decimal ? Price { get; set; }
        public string? CountryName { get; set; }
        public string? CityName { get; set; }
        public OrderStatusEnum Status { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<ProductQueryResponse> Products { get; set; }
    }
}
