using PriceHunter.Web.Helpers.Constants;

namespace PriceHunter.Web.Data.Filter
{
    public class FilterRequest
    {
        public List<FilterItem> AndFilters { get; set; } = new List<FilterItem>();
        public List<FilterItem> OrFilters { get; set; } = new List<FilterItem>();

        public Dictionary<string, OrderOperation> OrderOperations { get; set; } = new Dictionary<string, OrderOperation>();

        public int PageNumber { get; set; } = AppConstants.DefaultPageNumber;
        public int PageSize { get; set; } = AppConstants.DefaultPageSize;
    }
}
