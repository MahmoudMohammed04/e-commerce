using HomeCare.Models.ProductSchema;
using HomeCare.Services.Result;
using System.ComponentModel.DataAnnotations;

namespace HomeCare.Services.Products
{
    public interface IReviewService
    {
         Task<ServiceResult<Review>> AddReview(string userId, ReviewRequest request);
         Task<ServiceResult<List<ReviewResponse>>> GetProductReviews(int productId, int page = 1, int pageSize = 15);
    }

    public class ReviewRequest : IValidatableObject
    {
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Rating < 1 || Rating > 5)
                yield return new ValidationResult("Rating must be between 1 and 5");
        }

    }

    public class ReviewResponse
    {
        public int id { get; set; }
        public int rating { get; set; }
        public string? comment { get; set; }
        public string username { get; set; }
        public DateTime createdAt { get; set; }
    }
}
