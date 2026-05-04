using HomeCare.Services.Result;

namespace HomeCare.Services.Products
{
    public interface IFilterService
    {
       Task<ServiceResult<FilterResponse>> GetFilterAttributes(int categoryId);
    }

    public class FilterResponse
    {
        public List<FilterItem> Filters { get; set; } = new();
    }

    public class FilterItem
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
        public List<FilterValue> Values { get; set; } = new();
    }

    public class FilterValue
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
}
