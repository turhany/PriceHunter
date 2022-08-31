using PriceHunter.Business.TestData.Abstract;
using PriceHunter.Common.Data.Abstract;
using PriceHunter.Model.Supplier;

namespace PriceHunter.Business.TestData.Concrete
{
    public class TestDataService : ITestDataService
    {
        private readonly IGenericRepository<PriceHunter.Model.User.User> _userRepository;
        private readonly IGenericRepository<PriceHunter.Model.UserProduct.UserProduct> _userProductRepository;
        private readonly IGenericRepository<PriceHunter.Model.UserProduct.UserProductSupplierMapping> _userProductSupplierMappingRepository;
        private readonly IGenericRepository<PriceHunter.Model.Supplier.Supplier> _supplierRepository;
        private readonly IGenericRepository<PriceHunter.Model.Product.Product> _productRepository;
        private readonly IGenericRepository<PriceHunter.Model.Currency.Currency> _currencyRepository;
        IGenericRepository<PriceHunter.Model.Product.ProductSupplierInfoMapping> _productSupplierInfoMappingRepository;

        public TestDataService(
            IGenericRepository<Model.User.User> userRepository,
            IGenericRepository<PriceHunter.Model.UserProduct.UserProduct> userProductRepository,
            IGenericRepository<PriceHunter.Model.UserProduct.UserProductSupplierMapping> userProductSupplierMappingRepository,
            IGenericRepository<Model.Supplier.Supplier> supplierRepository,
            IGenericRepository<PriceHunter.Model.Product.Product> productRepository,
            IGenericRepository<PriceHunter.Model.Product.ProductSupplierInfoMapping> productSupplierInfoMappingRepository,
            IGenericRepository<PriceHunter.Model.Currency.Currency> currencyRepository
            )
        {
            _userRepository = userRepository;
            _userProductRepository = userProductRepository;
            _userProductSupplierMappingRepository = userProductSupplierMappingRepository;
            _supplierRepository = supplierRepository;
            _productRepository = productRepository;
            _productSupplierInfoMappingRepository = productSupplierInfoMappingRepository;
            _currencyRepository = currencyRepository;
        }

        public async Task InsertDataAsync(CancellationToken cancellationToken)
        {
            if (await _userRepository.AnyAsync(p => p.Email == "user@pricehunter.com", cancellationToken))
            {
                return;
            }

            var user = await InsertUserAsync(cancellationToken);
            var suppliers = await InsertSuppliersAsync(cancellationToken);
            var currencies = await InsertCurrenciesAsync(cancellationToken);
            var product = await InsertProductAsync(suppliers, currencies, cancellationToken);
            await InsertUserProductAsync(user, suppliers, currencies, product, cancellationToken);
        }


        private async Task<Model.User.User> InsertUserAsync(CancellationToken cancellationToken)
        {
            var user = new Model.User.User
            {
                FirstName = "User",
                LastName = "PriceHunter",
                Email = "user@pricehunter.com",
                Password = BCrypt.Net.BCrypt.HashPassword("123456789.tY"),
                Type = Model.User.UserType.Root
            };

            await _userRepository.InsertAsync(user, cancellationToken);
            return user;
        }
        private async Task<List<Model.Supplier.Supplier>> InsertSuppliersAsync(CancellationToken cancellationToken)
        {
            var supplierList = new List<Model.Supplier.Supplier>();
            supplierList.Add(new Model.Supplier.Supplier
            {
                Name = SupplierType.Amazon.ToString(),
                Order = 1,
                PriceControlPeriodAsMinute = 1,
                EnumMapping = SupplierType.Amazon.GetHashCode()
            });
            supplierList.Add(new Model.Supplier.Supplier
            {
                Name = SupplierType.Alibaba.ToString(),
                Order = 2,
                PriceControlPeriodAsMinute = 1,
                EnumMapping = SupplierType.Alibaba.GetHashCode()
            });
            supplierList.Add(new Model.Supplier.Supplier
            {
                Name = SupplierType.AliExpress.ToString(),
                Order = 3,
                PriceControlPeriodAsMinute = 1,
                EnumMapping = SupplierType.AliExpress.GetHashCode()
            });

            var suppliers = await _supplierRepository.InsertManyAsync(supplierList, cancellationToken);
            return suppliers;
        }
        private async Task<List<Model.Currency.Currency>> InsertCurrenciesAsync(CancellationToken cancellationToken)
        {
            var currencyList = new List<Model.Currency.Currency>();
            currencyList.Add(new Model.Currency.Currency
            {
                Name = "Dolar",
                ShortCode = "USD",
                Order = 1
            });
            currencyList.Add(new Model.Currency.Currency
            {
                Name = "Euro",
                ShortCode = "EU",
                Order = 2
            });
            currencyList.Add(new Model.Currency.Currency
            {
                Name = "Lira",
                ShortCode = "TL",
                Order = 3
            });

            var suppliers = await _currencyRepository.InsertManyAsync(currencyList, cancellationToken);
            return suppliers;
        }
        private async Task<Model.Product.Product> InsertProductAsync(List<Model.Supplier.Supplier> suppliers, List<Model.Currency.Currency> currencies, CancellationToken cancellationToken)
        {
            var product = await _productRepository.InsertAsync(new Model.Product.Product
            {
                Name = "Headphones",
                CurrencyId = currencies.First().Id
            }, cancellationToken);

            await _productSupplierInfoMappingRepository.InsertAsync(new Model.Product.ProductSupplierInfoMapping
            {
                ProductId = product.Id,
                SupplierId = suppliers.First().Id,
                Url = "https://www.amazon.com.tr/Logitech-LIGHTSPEED-kulakl%C4%B1%C4%9F%C4%B1-Teknolojisi-Hoparl%C3%B6rler/dp/B07W6FQ658/?_encoding=UTF8&pd_rd_w=iLJWl&content-id=amzn1.sym.8a1231b3-9dd1-4590-bc25-426daace92a4&pf_rd_p=8a1231b3-9dd1-4590-bc25-426daace92a4&pf_rd_r=30X37DGQX0WYDH83EXPP&pd_rd_wg=mbI3e&pd_rd_r=14e9fb5a-1d11-4240-991f-856395bdcdc7&ref_=pd_gw_crs_zg_bs_12466497031"
            }, cancellationToken);
            return product;
        }
        private async Task InsertUserProductAsync(Model.User.User user, List<Model.Supplier.Supplier> suppliers, List<Model.Currency.Currency> currencies, Model.Product.Product product, CancellationToken cancellationToken)
        {
            var userProduct = await _userProductRepository.InsertAsync(new Model.UserProduct.UserProduct
            {
                Name = product.Name,
                UserId = user.Id,
                CurrencyId = currencies.First().Id
            }, cancellationToken);

            await _userProductSupplierMappingRepository.InsertAsync(new Model.UserProduct.UserProductSupplierMapping
            {
                ProductId = product.Id,
                SupplierId = suppliers.First().Id,
                Url = "https://www.amazon.com.tr/Logitech-LIGHTSPEED-kulakl%C4%B1%C4%9F%C4%B1-Teknolojisi-Hoparl%C3%B6rler/dp/B07W6FQ658/?_encoding=UTF8&pd_rd_w=iLJWl&content-id=amzn1.sym.8a1231b3-9dd1-4590-bc25-426daace92a4&pf_rd_p=8a1231b3-9dd1-4590-bc25-426daace92a4&pf_rd_r=30X37DGQX0WYDH83EXPP&pd_rd_wg=mbI3e&pd_rd_r=14e9fb5a-1d11-4240-991f-856395bdcdc7&ref_=pd_gw_crs_zg_bs_12466497031",
                UserProductId = userProduct.Id
            }, cancellationToken);

            var userProduct2 = await _userProductRepository.InsertAsync(new Model.UserProduct.UserProduct
            {
                Name = product.Name + 2,
                UserId = user.Id,
                CurrencyId = currencies.Last().Id
            }, cancellationToken);

            await _userProductSupplierMappingRepository.InsertAsync(new Model.UserProduct.UserProductSupplierMapping
            {
                ProductId = product.Id,
                SupplierId = suppliers.First().Id,
                Url = "https://www.amazon.com.tr/Logitech-LIGHTSPEED-kulakl%C4%B1%C4%9F%C4%B1-Teknolojisi-Hoparl%C3%B6rler/dp/B07W6FQ658/?_encoding=UTF8&pd_rd_w=iLJWl&content-id=amzn1.sym.8a1231b3-9dd1-4590-bc25-426daace92a4&pf_rd_p=8a1231b3-9dd1-4590-bc25-426daace92a4&pf_rd_r=30X37DGQX0WYDH83EXPP&pd_rd_wg=mbI3e&pd_rd_r=14e9fb5a-1d11-4240-991f-856395bdcdc7&ref_=pd_gw_crs_zg_bs_12466497031",
                UserProductId = userProduct2.Id
            }, cancellationToken);
        }

    }
}
