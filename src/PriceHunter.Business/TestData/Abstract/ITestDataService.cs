using PriceHunter.Common.Data.Abstract; 

namespace PriceHunter.Business.TestData.Abstract
{
    public interface ITestDataService : IService
    {
        Task InsertDataAsync(CancellationToken cancellationToken);
    }
}
