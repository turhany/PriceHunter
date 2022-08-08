using PriceHunter.Common.Attributes;

namespace PriceHunter.Model.Supplier
{
    public enum SupplierType
    {
        [DatabaseId("00000000-0000-0000-0000-000000000000")]
        None = 0,
        [DatabaseId("557e8f56-bd01-4cad-afae-33fa051bdb56")]
        Amazon = 1,
        [DatabaseId("60f8868f-71cf-468f-9cc5-9ee44680dc6e")]
        Alibaba = 2,
        [DatabaseId("73289796-f9ca-4abd-acac-c4b05749576b")]
        AliExpress = 3
    }
}
