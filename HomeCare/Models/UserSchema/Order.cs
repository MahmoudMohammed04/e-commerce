using HomeCare.Models.LocationSchema;
using HomeCare.Models.ProductSchema;

namespace HomeCare.Models.UserSchema
{
    public class Order : IModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public decimal Price { get; set; }
        public OrderStatusEnum Status { get; set; } = OrderStatusEnum.Pending;
        public DateTime CreatedAt { get; set; }

        public int CountryId { get; set; }
        public Country Country { get; set; }

        public int CityId { get; set; }
        public City City { get; set; }

        public string? PostalCode { get; set; }
        public string FullName { get; set; }
        public string  PhoneNumber { get; set; }
        public string Email { get; set; }
        public string StreetAddress { get; set; }
        public string? State { get; set; }

        public List<OrderProduct> OrderProducts { get; set; }
    }
}
