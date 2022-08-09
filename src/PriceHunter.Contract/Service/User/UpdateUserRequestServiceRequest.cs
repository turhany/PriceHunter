using Microsoft.AspNetCore.Http;
using PriceHunter.Model.User;

namespace PriceHunter.Contract.Service.User
{
    public class UpdateUserRequestServiceRequest
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IFormFile Image { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserType Type { get; set; }
        public bool IsActive { get; set; }
    }
}