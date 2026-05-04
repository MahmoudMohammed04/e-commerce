namespace HomeCare.Models.ProductSchema
{
    public class ProductImage:IModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Image { get; set; }

        public Product Product { get; set; }
    }
}
