using Microsoft.AspNetCore.Http;

namespace PriceHunter.Contract.App.UserProduct
{
    public class UpdateUserProductRequest
    {
        public string Name { get; set; }
        public IFormFile Image { get; set; }
        public List<UrlSupplierMappingViewModel> UrlSupplierMapping { get; set; }
    }
}
