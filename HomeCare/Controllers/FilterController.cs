using HomeCare.Context;
using HomeCare.Extentions;
using HomeCare.Models.ProductSchema;
using HomeCare.Services.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class FilterController : Controller
    {
        private readonly IFilterService _filterService;
        private readonly AppDbContext _context;
        public FilterController(IFilterService filterService , AppDbContext context)
        {
            _filterService = filterService;
            _context = context;
        }

        [HttpGet("GetFilter")]
        public async Task<ActionResult<FilterResponse>> GetFilter([FromQuery] int categoryId)
        {
            var result = await _filterService.GetFilterAttributes(categoryId);
            return result.Success ? Ok(result.Data) : result.ErrorToGenericActionResult();
        }

        [HttpGet("GetCategories")]
        public async Task<ActionResult<List<Category>>> GetCategory()
        {
            
            return Ok(await _context.Categories.Select(x => new { x.Id, x.Name }).ToListAsync());
        }

        [HttpGet("GetBrands")]
        public async Task<ActionResult<List<Brand>>> GetBrand([FromQuery] int CategoryId)
        {

            return Ok(await _context.Brands.Where(x => x.CategoryID == CategoryId).Select(x => new { x.Id, x.Name }).ToListAsync());
        }
    }
}
