using HomeCare.Context;
using HomeCare.Models.ProductSchema;
using HomeCare.Services.Result;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Repositories
{
    public class FacetIndexRepository:Repository<FacetIndexTable,int>
    {
      
        public FacetIndexRepository(AppDbContext context) : base(context)
        {
            
        }

        public async Task AddProductFacet(Product product)
        {
            var attributeValueIds = product.ProductAttributes
                .Select(x => x.AttributeValueId)
                .ToList();

            var attributeValues = await _context.AttributeValues
                .Where(x => attributeValueIds.Contains(x.id))
                .ToListAsync();

            var facetIndexTable = attributeValues.Select(attributeValue => new FacetIndexTable
            {
                ProductId = product.Id,
                AttributeId = attributeValue.attributeId,
                AttributeValueString = attributeValue.value,
                AttributeValueNumeric = attributeValue.numericValue
            }).ToList();

            await Table.AddRangeAsync(facetIndexTable); 
            await _context.SaveChangesAsync();         
        }
    }
}
