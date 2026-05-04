namespace HomeCare.Models.ProductSchema
{
    public class AttributeValue:IModel
    {
        public int id { get; set; }
        public string value { get; set; }

        public decimal? numericValue { get; set; }

        public int attributeId { get; set; }
        public AttributeType Attribute { get; set; }

        public List<ProductAttribute>? ProductAttributes { get; set; } = new List<ProductAttribute>();
        
    }
}
