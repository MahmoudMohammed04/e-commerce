using HomeCare.Context;
using HomeCare.Models.UserSchema;
using HomeCare.Services.Result;
using HomeCare.Services.Url;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Services.Products
{
    public class OrderService : IOrderService
    {

        private readonly AppDbContext _context;
        private readonly IUrlService _urlService;
        public OrderService(AppDbContext context, IUrlService urlService)
        {
            _context = context;
            _urlService = urlService;
        }

        public async Task<ServiceResult<Order>> CreateOrder(string userId, CreateOrderRequest request)
        {
            var productIds = request.Products.Select(p => p.ProductId).ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            foreach (var item in request.Products)
            {
                if (!products.ContainsKey(item.ProductId))
                    return new ServiceResult<Order>("Product not found", ErrorTypeEnum.NOT_FOUND);

                var product = products[item.ProductId];

                if (product.Quantity < item.Quantity)
                    return new ServiceResult<Order>(
                        $"Not enough stock for {product.Name}",
                        ErrorTypeEnum.CONFLICT);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    UserId = userId,
                    FullName = request.FullName,
                    PhoneNumber = request.PhoneNumber,
                    Email = request.Email,
                    CityId = request.cityId,
                    CountryId = request.countryId,
                    PostalCode = request.PostalCode,
                    State = request.state,
                    Status = OrderStatusEnum.Pending,
                    StreetAddress = request.StreetAddress,
                    CreatedAt = DateTime.UtcNow,
                    Price = request.Products.Sum(p => p.Quantity * products[p.ProductId].Price),

                    OrderProducts = request.Products.Select(p => new OrderProduct
                    {
                        ProductId = p.ProductId,
                        Quantity = p.Quantity,
                        UnitPrice = products[p.ProductId].Price
                    }).ToList()
                };

                await _context.Orders.AddAsync(order);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return new ServiceResult<Order>(order);
            }
            catch (Exception ex) {
                await transaction.RollbackAsync();
                return new ServiceResult<Order>(ex.Message, ErrorTypeEnum.SERVER_ERROR);
            }

        }
        public async Task<ServiceResult<List<OrderQueryResponse>>> GetOrders(string userId, int page = 1, int pageSize = 15)
        {

            var orders = await _context.Orders
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderQueryResponse
                {
                    Address = o.StreetAddress,
                    CreatedDate = o.CreatedAt,
                    Status = o.Status,
                    Id = o.Id
                })
                .ToListAsync();

            if (orders.Count == 0)
                return new ServiceResult<List<OrderQueryResponse>>("Orders not found", ErrorTypeEnum.NOT_FOUND);
            else
            {
                return new ServiceResult<List<OrderQueryResponse>>(orders);
            }
            
        }

        public async Task<ServiceResult<List<ProductQueryResponse>>> GetOrderProducts(string userId, int orderId)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
                return new ServiceResult<List<ProductQueryResponse>>("Order not found", ErrorTypeEnum.NOT_FOUND);

            var products = order.OrderProducts.Select(op => op.Product).ToList();

            string baseUrl = _urlService.GetBaseUrl();
            var response = products.Select(p => new ProductQueryResponse
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryName = p.Category.Name,
                Image = baseUrl + p.Image,
                Description = p.Description,
                BrandName = p.Brand.Name
            }).ToList();

            return new ServiceResult<List<ProductQueryResponse>>(response);
        }

        public async Task<ServiceResult<OrderDetailsResponse>> GetOrderDetails(string userId, int orderId)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .Include(o=>o.Country)
                .Include(o => o.City)
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .ThenInclude(p => p.Category)
                .ThenInclude(c => c.Brands)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return new ServiceResult<OrderDetailsResponse>("Order not found", ErrorTypeEnum.NOT_FOUND);

            var OrderDetails = new OrderDetailsResponse()
            {
                Id = order.Id,
                FullName = order.FullName,
                PhoneNumber = order.PhoneNumber,
                Email = order.Email,
                CityName = order.City.Name,
                CountryName = order.Country.Name,
                PostalCode = order.PostalCode,
                state = order.State,
                StreetAddress = order.StreetAddress,
                CreatedDate = order.CreatedAt,
                Price = order.Price,
                Status = order.Status,
                Products = order.OrderProducts.Select(op => new ProductQueryResponse
                {
                    Id = op.Product.Id,
                    Name = op.Product.Name,
                    Price = op.Product.Price,
                    CategoryName = op.Product.Category.Name,
                    Image = _urlService.GetBaseUrl() + op.Product.Image,
                    Description = op.Product.Description,
                    BrandName = op.Product.Brand.Name
                }).ToList()
            };

            return new ServiceResult<OrderDetailsResponse>(OrderDetails);

        }
        public async Task<ServiceResult<Order>> GetOrder(int orderId)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return new ServiceResult<Order>("Order not found", ErrorTypeEnum.NOT_FOUND);

            return new ServiceResult<Order>(order);
        }

        public async Task<ServiceResult<bool>> UpdateOrderToPiad(int orderId)
        {
            var order = await _context.Orders
               .Include(o => o.OrderProducts)
               .ThenInclude(op => op.Product)
               .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return new ServiceResult<bool>("Order not found", ErrorTypeEnum.NOT_FOUND);

            if(order.Status != OrderStatusEnum.Pending)
                return new ServiceResult<bool>("Order is not pending", ErrorTypeEnum.CONFLICT);

            order.Status = OrderStatusEnum.Paid;

            foreach (var item in order.OrderProducts)
            {
                var product = item.Product;

                product.Quantity -= item.Quantity;
                product.NumberOfSold += item.Quantity;

                if (product.Quantity <= 0)
                    product.Quantity = 0;

                _context.Products.Update(product);
            }

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return new ServiceResult<bool>(true);
        }

        public async Task<ServiceResult<bool>> CancelOrder(string userId,int orderId)
        {
            var order = await _context.Orders
               .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
                return new ServiceResult<bool>("Order not found", ErrorTypeEnum.NOT_FOUND);

            if (order.Status != OrderStatusEnum.Pending)
                return new ServiceResult<bool>("Order is not pending", ErrorTypeEnum.CONFLICT);

            order.Status = OrderStatusEnum.Cancelled;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return new ServiceResult<bool>(true);

        }
    }
}
