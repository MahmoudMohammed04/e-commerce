namespace HomeCare.Models.ProductSchema
{
    public class ProductAttribute : IModel
    {
        public int ProductId { get; set; }
        public int AttributeValueId { get; set; }

        public Product Product { get; set; }
        public AttributeValue AttributeValue { get; set; }
    }
}
