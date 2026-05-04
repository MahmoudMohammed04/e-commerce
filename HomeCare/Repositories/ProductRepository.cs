using HomeCare.Context;
using HomeCare.Models.ProductSchema;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Repositories
{
    public class ProductRepository : Repository<Product, int>
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    

       
    }
}
