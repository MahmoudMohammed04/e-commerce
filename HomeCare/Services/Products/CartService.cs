using HomeCare.Context;
using HomeCare.Models.UserSchema;
using HomeCare.Services.PaymentService;
using HomeCare.Services.Result;
using HomeCare.Services.Url;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HomeCare.Services.Products
{
    public class CartService : ICartService
    {

        private readonly AppDbContext _context;
        private readonly IOrderService _orderService;
        private readonly IUrlService _urlService;
        private readonly IPaymentService _paymentService;

        public CartService(AppDbContext context, IOrderService orderService, IUrlService urlService , IPaymentService paymentService)
        {
            _context = context;
            _orderService = orderService;
            _urlService = urlService;
            _paymentService = paymentService;
        }


        public async Task<ServiceResult<bool>> AddToCart(string userId,AddToCartRequest request)
        {
            if (request.quantity <= 0)
                return new ServiceResult<bool>("Invalid quantity", ErrorTypeEnum.BAD_REQUEST);

            var product = await _context.Products.FindAsync(request.productId);

            if (product == null)
                return new ServiceResult<bool>("Product not found", ErrorTypeEnum.NOT_FOUND);

            var CartItem = await _context.Carts.FirstOrDefaultAsync(c => c.ProductId == request.productId && c.UserId == userId);

            if (CartItem != null)
            {
                CartItem.quantity += request.quantity;
                _context.Carts.Update(CartItem);
                await _context.SaveChangesAsync();
                return new ServiceResult<bool>(true);
            }


            try
            {
                var cart = new Cart
                {
                    UserId = userId,
                    ProductId = request.productId,
                    quantity = request.quantity,
                    UnitPrice = product.Price
                    
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();

                return new ServiceResult<bool>(true);
            }
            catch (Exception ex) {
                return new ServiceResult<bool>(ex.Message, ErrorTypeEnum.SERVER_ERROR);
            }

        }

        public async Task<ServiceResult<bool>> RemoveFromCart(string userId,RemoveFromCartRequest request)
        {
           
            try
            {
                var cart = await _context.Carts
                    .FirstOrDefaultAsync(c => c.ProductId == request.ProductId && c.UserId == userId);

                if (cart == null)
                    return new ServiceResult<bool>("Cart item not found", ErrorTypeEnum.NOT_FOUND);

                if (cart != null)
                {
                    _context.Carts.Remove(cart);
                    await _context.SaveChangesAsync();
                }
                return new ServiceResult<bool>(true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool>(ex.Message, ErrorTypeEnum.SERVER_ERROR);
            }
            
        }

        public async Task<ServiceResult<List<ProductQueryResponse>>> ShowCart(string userId,int page = 1, int pageSize = 15)
        {
            var cartItems = await _context.Carts
                .AsNoTracking()
                .Include(c => c.Product)
                .ThenInclude(p => p.Category)
                .Include(c => c.Product)
                .ThenInclude(p => p.Brand)
                .Where(c => c.UserId == userId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (cartItems.Count == 0)
                return new ServiceResult<List < ProductQueryResponse >> ("Cart is empty", ErrorTypeEnum.NOT_FOUND);

            string baseUrl = _urlService.GetBaseUrl();
            var products = cartItems.Select(c => c.Product).ToList().Select(p => new ProductQueryResponse
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                BrandName = p.Brand.Name,
                CategoryName = p.Category.Name,
                Description = p.Description,
                Image = baseUrl + p.Image,
                
            }).ToList();

            return new ServiceResult<List <ProductQueryResponse>> (products);
        }

        public async Task<ServiceResult<bool>> ClearCart(string userId)
        {
            var cartItems = await _context.Carts.Where(c => c.UserId == userId).ToListAsync();

            if (cartItems.Count == 0)
                return new ServiceResult<bool>("Cart is empty", ErrorTypeEnum.NOT_FOUND);

            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return new ServiceResult<bool>(true);
        }

        public async Task<ServiceResult<string>> CheckOut(string userId,CheckOutRequest request)
        {
            var order = new CreateOrderRequest()
            {
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                FullName = request.FullName,
                cityId = request.cityId,
                countryId = request.countryId,
                PostalCode = request.PostalCode,
                state = request.state,
                StreetAddress = request.StreetAddress,
                Products = await _context.Carts.Where(c => c.UserId == userId).Select(c => new OrderProductDetials()
                {
                    ProductId = c.ProductId,
                    Quantity = c.quantity

                }).ToListAsync()

            };
            var result = await _orderService.CreateOrder(userId, order);

            if (!result.Success)
                return new ServiceResult<string>(result.ErrorMessage, ErrorTypeEnum.CONFLICT);

            var PaymentIntent = new PaymentIntentRequest()
            {
                PaymentRequest = request.PaymentRequest,
                Amount = result.Data.Price,
                OrderId = result.Data.Id
                

            };

            var PayMintResult = await _paymentService.CreatePaymentIntent(userId,PaymentIntent);

            if (!PayMintResult.Success)
                return new ServiceResult<string>(PayMintResult.ErrorMessage, ErrorTypeEnum.CONFLICT);




            return PayMintResult;
        }
    }
}
