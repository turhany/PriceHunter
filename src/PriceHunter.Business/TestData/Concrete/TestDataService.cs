using PriceHunter.Business.TestData.Abstract;
using PriceHunter.Common.Data.Abstract;

namespace PriceHunter.Business.TestData.Concrete
{
    public class TestDataService : ITestDataService
    {
        private readonly IGenericRepository<PriceHunter.Model.User.User> _userRepository;
        private readonly IGenericRepository<PriceHunter.Model.Supplier.Supplier> _supplierRepository;

        public TestDataService(
            IGenericRepository<Model.User.User> userRepository,
            IGenericRepository<Model.Supplier.Supplier> supplierRepository
            )
        {
            _userRepository = userRepository;
            _supplierRepository = supplierRepository;
        }

        public async Task InsertDataAsync()
        {
            var entity = new Model.User.User
            {
                FirstName = "Türhan",
                LastName = "Yıldırım",
                Email = "yildirimturhan@gmail.com",
                Password = BCrypt.Net.BCrypt.HashPassword("123456789.tY"),
                Type = Model.User.UserType.Root
            };

            await _userRepository.InsertAsync(entity);

            var supplierList = new List<Model.Supplier.Supplier>();
            supplierList.Add(new Model.Supplier.Supplier
            {
                Name = "Amazon",
                Order = 1
            });
            supplierList.Add(new Model.Supplier.Supplier
            {
                Name = "Alibaba",
                Order = 2
            });
            supplierList.Add(new Model.Supplier.Supplier
            {
                Name = "AliExpress",
                Order = 3
            });

            await _supplierRepository.InsertManyAsync(supplierList);
        }
    }
}
