using PriceHunter.Common.Data; 

namespace PriceHunter.Model.UserProduct
{
    public class UserProduct : SoftDeleteEntity
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public Guid CurrencyId { get; set; }
        public bool IsFavorite { get; set; }
    }
}
