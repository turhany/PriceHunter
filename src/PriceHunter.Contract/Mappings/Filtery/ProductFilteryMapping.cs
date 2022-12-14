using Filtery.Configuration.Filtery;
using Filtery.Constants;
using Filtery.Models.Filter;
using PriceHunter.Model.Product;

namespace PriceHunter.Contract.Mappings.Filtery;

public class ProductFilteryMapping: AbstractFilteryMapping<Product>
{
    public ProductFilteryMapping()
    {
        mapper
            .Name("name")
            .OrderProperty(p =>p.Name)
            .Filter(p => p.Name.ToLower().Equals(FilteryQueryValueMarker.FilterStringValue.ToLower()), FilterOperation.Equal)
            .Filter(p => !p.Name.ToLower().Equals(FilteryQueryValueMarker.FilterStringValue.ToLower()), FilterOperation.NotEqual)
            .Filter(p => p.Name.ToLower().Contains(FilteryQueryValueMarker.FilterStringValue.ToLower()), FilterOperation.Contains)
            .Filter(p => p.Name.ToLower().StartsWith(FilteryQueryValueMarker.FilterStringValue.ToLower()), FilterOperation.StartsWith)
            .Filter(p => p.Name.ToLower().EndsWith(FilteryQueryValueMarker.FilterStringValue.ToLower()), FilterOperation.EndsWith);
    }
}