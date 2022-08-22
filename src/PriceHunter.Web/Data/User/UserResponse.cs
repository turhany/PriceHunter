namespace PriceHunter.Web.Data.User
{
    [Serializable]
    public class UserResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Image { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
    }
}
