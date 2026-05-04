using HomeCare.Context;
using HomeCare.Models.ProductSchema;

namespace HomeCare.Repositories
{
    public class AttributeValueRepository:Repository<AttributeValue,int>
    {
        private readonly AppDbContext _context;

        public AttributeValueRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
