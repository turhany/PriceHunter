using PriceHunter.Common.Data; 

namespace PriceHunter.Model.User
{
    public class User : SoftDeleteEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Image { get; set; }
    }
}
