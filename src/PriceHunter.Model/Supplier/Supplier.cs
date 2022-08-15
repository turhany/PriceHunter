using PriceHunter.Common.Data; 

namespace PriceHunter.Model.Supplier
{
    public class Supplier : SoftDeleteEntity
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public int PriceControlPeriodAsMinute { get; set; }
        public int EnumMapping { get; set; }
    }
}
