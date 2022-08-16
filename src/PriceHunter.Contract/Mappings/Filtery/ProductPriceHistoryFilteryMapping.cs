using Filtery.Configuration.Filtery;
using Filtery.Constants;
using Filtery.Models.Filter;
using PriceHunter.Model.Product;

namespace PriceHunter.Contract.Mappings.Filtery;

public class ProductPriceHistoryFilteryMapping : AbstractFilteryMapping<ProductPriceHistory>
{
    public ProductPriceHistoryFilteryMapping()
    {
        mapper
            .Name("price")
            .OrderProperty(p => p.Price)
            .Filter(p => p.Price == FilteryQueryValueMarker.FilterIntValue, FilterOperation.Equal)
            .Filter(p => p.Price != FilteryQueryValueMarker.FilterIntValue, FilterOperation.NotEqual)
            .Filter(p => p.Price > FilteryQueryValueMarker.FilterIntValue, FilterOperation.GreaterThan)
            .Filter(p => p.Price < FilteryQueryValueMarker.FilterIntValue, FilterOperation.LessThan)
            .Filter(p => p.Price >= FilteryQueryValueMarker.FilterIntValue, FilterOperation.GreaterThanOrEqual)
            .Filter(p => p.Price <= FilteryQueryValueMarker.FilterIntValue, FilterOperation.LessThanOrEqual);

        mapper
            .Name("supplierid")
            .OrderProperty(p => p.SupplierId)
            .Filter(p => p.SupplierId == FilteryQueryValueMarker.FilterGuidValue, FilterOperation.Equal)
            .Filter(p => p.SupplierId != FilteryQueryValueMarker.FilterGuidValue, FilterOperation.NotEqual);

        mapper
            .Name("supplierid")
            .OrderProperty(p => p.ProductId)
            .Filter(p => p.ProductId == FilteryQueryValueMarker.FilterGuidValue, FilterOperation.Equal)
            .Filter(p => p.ProductId != FilteryQueryValueMarker.FilterGuidValue, FilterOperation.NotEqual);
    }
}