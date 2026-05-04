namespace HomeCare.Models.ProductSchema
{
    public class FacetIndexTable:IModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int AttributeId { get; set; }
        public AttributeType AttributeType { get; set; }

        public string? AttributeValueString { get; set; }
        public decimal? AttributeValueNumeric { get; set; }
    }
}
