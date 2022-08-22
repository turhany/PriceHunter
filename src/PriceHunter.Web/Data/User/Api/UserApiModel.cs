namespace PriceHunter.Web.Data.User.Api
{
    [Serializable]
    public class UserApiModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Image { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public string Password { get; set; }
    }
}
