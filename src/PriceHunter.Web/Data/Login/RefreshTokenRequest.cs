using System.ComponentModel.DataAnnotations;

namespace PriceHunter.Web.Data.Login
{
    public class RefreshTokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
