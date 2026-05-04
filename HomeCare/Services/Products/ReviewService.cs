using HomeCare.Context;
using HomeCare.Models.ProductSchema;
using HomeCare.Services.Result;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Services.Products
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;

        public ReviewService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<Review>> AddReview(string userId,ReviewRequest request)
        {

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var product = await _context.Products.FindAsync(request.ProductId);

                if (product == null)
                    return new ServiceResult<Review>("Product not found", ErrorTypeEnum.NOT_FOUND);

                var review = new Review
                {
                    ProductId = request.ProductId,
                    UserId = userId,
                    Rating = request.Rating,
                    Comment = request.Comment,
                    CreatedAt = DateTime.UtcNow
                };

                product.AverageRating = ((product.AverageRating * product.RatingCount) + request.Rating)
    / (product.RatingCount + 1);

                product.RatingCount++;

                _context.Reviews.Add(review);
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ServiceResult<Review>(review);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ServiceResult<Review>(ex.Message, ErrorTypeEnum.SERVER_ERROR);
            }
        }

        public async Task<ServiceResult<List<ReviewResponse>>> GetProductReviews(int productId , int page = 1, int pageSize = 15)
        {
            var reviews = await _context.Reviews
                .AsNoTracking()
                .Include(r => r.User)
                .Where(p => p.ProductId == productId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new ServiceResult<List<ReviewResponse>>(reviews.Select(r => new ReviewResponse
            {
                id = r.Id,
                username = r.User.UserName,
                rating = r.Rating,
                comment = r.Comment,
                createdAt = r.CreatedAt
            }).ToList());
        }
    }
}
