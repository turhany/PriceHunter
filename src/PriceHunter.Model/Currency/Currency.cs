using PriceHunter.Common.Data; 

namespace PriceHunter.Model.Currency
{
    public class Currency : SoftDeleteEntity
    {
        public string Name { get; set; }
        public string ShortCode { get; set; }
        public int Order { get; set; }
    }
}
