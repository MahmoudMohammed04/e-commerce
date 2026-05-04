using HomeCare.Context;
using HomeCare.Extentions;
using HomeCare.Models.ProductSchema;
using HomeCare.Repositories;
using HomeCare.Services.FileService;
using HomeCare.Services.Result;
using HomeCare.Services.Url;
using Microsoft.EntityFrameworkCore;
using static HomeCare.Services.Products.ProductQueryRequest;

namespace HomeCare.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly ProductRepository _productRepository;
        private readonly FacetIndexRepository _facetIndexRepository;
        private readonly IFileService _fileService;
        private readonly IUrlService _urlService;
        private readonly string _imagesFolder = "product";
        public ProductService(ProductRepository productRepository,
            AttributeValueRepository attributeValueRepository,
            FacetIndexRepository facetIndexRepository,
            IFileService fileService,
            IUrlService urlService)
        {
            _productRepository = productRepository;
            _facetIndexRepository = facetIndexRepository;
            _fileService = fileService;
            _urlService = urlService;
        }

        public async Task<ServiceResult<bool>> AddProduct(AddProductRequest request)
        {
            if (request == null)
                return new ServiceResult<bool>("Invalid request", ErrorTypeEnum.BAD_REQUEST);

           

            using var transaction = _productRepository.GetTransaction();

            try
            {
                var product = new Product
                {
                    Name = request.name,
                    Description = request.description,
                    Price = request.price,
                    Quantity = request.quantity,
                    CategoryId = request.categoryId,
                    Image = "",
                    BrandId = request.BrandId,

                    
                };

                await _productRepository.AddAsync(product);
                //await _productRepository.SaveAsync(); 

                product.ProductAttributes = request.attributes.Select(attrId =>
                    new ProductAttribute
                    {
                        ProductId = product.Id,        
                        AttributeValueId = attrId
                    }).ToList();

                await _productRepository.SaveAsync();

                //await _facetIndexRepository.AddProductFacet(product);


                var imageResult = await _fileService.SaveFileAsync(request.image, _imagesFolder);
                if (!imageResult.Success)
                    return new ServiceResult<bool>(imageResult.ErrorMessage, ErrorTypeEnum.CONFLICT);

                var imagesResult = await _fileService.SaveFileAsync(request.images, _imagesFolder);
                if (!imagesResult.Success)
                    return new ServiceResult<bool>(imagesResult.ErrorMessage, ErrorTypeEnum.CONFLICT);

                Console.WriteLine(imageResult.Data + " " + imagesResult.Data.FilePaths.Count);
                product.Image = imageResult.Data;
                product.ProductImages = imagesResult.Data.FilePaths.Select(pi => new ProductImage { ProductId = product.Id, Image = pi }).ToList();
                await _productRepository.SaveAsync();

                transaction.Commit();
                return new ServiceResult<bool>(true);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new ServiceResult<bool>(ex.Message, ErrorTypeEnum.CONFLICT);
            }
        }

        public async Task<ServiceResult<ProductDetailsResponse>> GetProductDetails(int id)
        {
            var product = await _productRepository.Table
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.ProductImages)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return new ServiceResult<ProductDetailsResponse>("Product not found", ErrorTypeEnum.NOT_FOUND);

            string baseUrl = _urlService.GetBaseUrl();

            var response = new ProductDetailsResponse
            {
                
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                CategoryName = product.Category.Name,
                Image = baseUrl + product.Image,
                Images = product.ProductImages.Select(pi => baseUrl + pi.Image).ToList(),
                BrandName = product.Brand.Name,
                MarchantName = product.User.MarhcantName
            };

            return new ServiceResult<ProductDetailsResponse>(response);
        }
        public async Task<ServiceResult<bool>> DeleteProduct(int id)
        {
            if (id <= 0)
                return new ServiceResult<bool>("Invalid request", ErrorTypeEnum.BAD_REQUEST);

            var product = await _productRepository.Table.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);

            var deleteResult =  _fileService.DeleteFileAsync(product.Image);

            if (!deleteResult.Success) {
                return new ServiceResult<bool>(deleteResult.ErrorMessage, ErrorTypeEnum.CONFLICT);
            }

            var deleteImagesResult =  _fileService.DeleteFileAsync(product.ProductImages.Select(pi => pi.Image).ToList());

            if (!deleteImagesResult.Success) {
                return new ServiceResult<bool>(deleteImagesResult.ErrorMessage, ErrorTypeEnum.CONFLICT);
            }

            if(product == null)
                return new ServiceResult<bool>("Product not found", ErrorTypeEnum.NOT_FOUND);

            _productRepository.Delete(product);
            await _productRepository.SaveAsync();


            return new ServiceResult<bool>(true);
        }

        public async Task<ServiceResult<List<ProductQueryResponse>>> GetProducts(ProductQueryRequest request,int page = 1, int pageSize = 15)
        {
            if (page <= 0 || pageSize <= 0)
                return new ServiceResult<List<ProductQueryResponse>>("Invalid pagination", ErrorTypeEnum.BAD_REQUEST);

            var query = _productRepository.Table.AsQueryable();

           query = query
                .WhereIf(request.categoryId.HasValue, p => p.CategoryId == request.categoryId)
                .WhereIf(request.brandId.HasValue, p => p.BrandId == request.brandId)
                .WhereIf(request.minPrice.HasValue, p => p.Price >= request.minPrice)
                .WhereIf(request.maxPrice.HasValue, p => p.Price <= request.maxPrice);

            if(request.attributeValueIds != null && request.attributeValueIds.Count > 0)
            {
                query = query.Where(p =>
                    request.attributeValueIds.All(attrId =>
                        p.ProductAttributes.Any(pa => pa.AttributeValueId == attrId)
                    )
                );
            }

            string baseUrl = _urlService.GetBaseUrl();
            var products = await query
                .AsNoTracking()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductQueryResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Image = baseUrl + p.Image,
                    CategoryName = p.Category.Name,
                    BrandName = p.Brand.Name,
                    Sold = p.NumberOfSold,

                })
                .ToListAsync();

            return new ServiceResult<List<ProductQueryResponse>>(products);
        }

    

    }
}
