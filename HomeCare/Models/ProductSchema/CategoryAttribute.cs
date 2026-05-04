namespace HomeCare.Models.ProductSchema
{
    public class CategoryAttribute:IModel
    {
        public int CategoryId { get; set; }
        public int AttributeId { get; set; }

        public Category Category { get; set; }
        public AttributeType Attribute { get; set; }
    }
}
