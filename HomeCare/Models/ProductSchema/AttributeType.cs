namespace HomeCare.Models.ProductSchema
{
    public class AttributeType:IModel
    {
        public int id { get; set; }
        public string name { get; set; }

        public List<AttributeValue>? attributeValues { get; set; } = new List<AttributeValue>();

        public List<CategoryAttribute>? categoryAttributes { get; set; } = new List<CategoryAttribute>();
        public List<FacetIndexTable>? FacetIndexTables { get; set; } = new List<FacetIndexTable>();
    }
}
