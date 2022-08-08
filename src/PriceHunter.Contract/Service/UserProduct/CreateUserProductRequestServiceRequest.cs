using Microsoft.AspNetCore.Http;

namespace PriceHunter.Contract.Service.UserProduct
{
    public class CreateUserProductRequestServiceRequest
    {
        public string Name { get; set; }
        public IFormFile Image { get; set; }
        public List<UrlSupplierMappingServiceModel> UrlSupplierMapping { get; set; }
    }
}
