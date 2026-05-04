using HomeCare.Models.ProductSchema;
using HomeCare.Services.Result;
using System.ComponentModel.DataAnnotations;
using static HomeCare.Services.Products.ProductQueryRequest;

namespace HomeCare.Services.Products
{
    public interface IProductService
    {
        Task<ServiceResult<bool>> AddProduct(AddProductRequest request);
        Task<ServiceResult<bool>> DeleteProduct(int id);
        Task<ServiceResult<ProductDetailsResponse>> GetProductDetails(int id);
        Task<ServiceResult<List<ProductQueryResponse>>> GetProducts(ProductQueryRequest request, int page = 1, int pageSize = 15);

      
        
    }

    public class AddProductRequest
    {
        public string name { get; set; }
        public string description { get; set; }
        public int categoryId { get; set; }
        public IFormFile image { get; set; }  //to image file laster

        public List<IFormFile> images { get; set; } = new List<IFormFile>(); //to image file laster>

        public int BrandId { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }

        public List<int> attributes { get; set; } = new();

    }

    public class ProductQueryRequest : IValidatableObject
    {
        public int? categoryId { get; set; }
        public int? brandId { get; set; }

        public decimal? minPrice { get; set; }
        public decimal? maxPrice { get; set; }

        public List<int> attributeValueIds { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (minPrice != null && maxPrice != null && minPrice > maxPrice)
                yield return new ValidationResult("Min price must be less than max price");

            
        }


    }

    public class ProductQueryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Sold { get; set; }
        public string Image { get; set; }
        public string? CategoryName { get; set; }
        public string? BrandName { get; set; }
    }

    public class ProductDetailsResponse
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Image { get; set; }
        public string? CategoryName { get; set; }
        public string? BrandName { get; set; }
        public List<string> Images { get; set; } = new();
        public string? MarchantName { get; set; }
    }

   
}
