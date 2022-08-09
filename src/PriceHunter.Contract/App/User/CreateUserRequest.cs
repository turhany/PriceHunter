using Microsoft.AspNetCore.Http;

namespace PriceHunter.Contract.App.User
{
    public class CreateUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IFormFile Image { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}