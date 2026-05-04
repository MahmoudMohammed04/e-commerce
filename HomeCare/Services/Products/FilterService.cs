using HomeCare.Context;
using HomeCare.Services.Result;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Services.Products
{
    public class FilterService: IFilterService
    {
        private readonly AppDbContext _context;

        public FilterService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<FilterResponse>> GetFilterAttributes(int categoryId)
        {
            var filters = await _context.CategoryAttributes
                .AsNoTracking()
                .Where(ca => ca.CategoryId == categoryId)
                .Join(_context.AttributeTypes,
                    ca => ca.AttributeId,
                    at => at.id,
                    (ca, at) => new { ca, at })
                .GroupJoin(
                    _context.AttributeValues,
                    x => x.ca.AttributeId,
                    av => av.attributeId,
                    (x, values) => new FilterItem
                    {
                        AttributeId = x.ca.AttributeId,
                        AttributeName = x.at.name,
                        Values = values.Select(v => new FilterValue
                        {
                            Id = v.id,
                            Value = v.value
                        }).ToList()
                    })
                .ToListAsync();

            return new ServiceResult<FilterResponse>(new FilterResponse
            {
                Filters = filters
            });
        }
    }
}
