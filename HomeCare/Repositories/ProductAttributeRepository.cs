using HomeCare.Context;
using Microsoft.AspNetCore.Mvc;

namespace HomeCare.Repositories
{
    public class ProductAttributeRepository:Repository<ProducesAttribute,int>
    {
        private readonly AppDbContext _context;
        public ProductAttributeRepository(AppDbContext context):base(context)
        {
            _context = context;
        }


    }
}
