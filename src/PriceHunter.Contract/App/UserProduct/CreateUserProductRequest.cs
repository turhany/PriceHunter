using Microsoft.AspNetCore.Http;

namespace PriceHunter.Contract.App.UserProduct
{
    public class CreateUserProductRequest
    {
        public string Name { get; set; }
        public List<UrlSupplierMappingViewModel> UrlSupplierMapping { get; set; }
    }
}
