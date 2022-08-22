using System.ComponentModel.DataAnnotations;

namespace PriceHunter.Web.Data.Login
{
    [Serializable]
    public class GetTokenRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
