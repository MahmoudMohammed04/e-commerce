using HomeCare.Context;
using HomeCare.Models.LocationSchema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class LocationController : Controller
    {
        private readonly AppDbContext _context;
        public LocationController( AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetCities")]
        public async Task<ActionResult<List<City>>> GetCities([FromQuery] int CountryId)
        {
            return Ok(await _context.Cities.Where(x => x.CountryId == CountryId).Select(x => new { x.Id, x.Name }).ToListAsync());
        }

        [HttpGet("GetCountries")]
        public async Task<ActionResult<List<Country>>> GetCountries()
        {
            return Ok(await _context.Countries.Select(x => new { x.Id, x.Name }).ToListAsync());
        }
    }
}
