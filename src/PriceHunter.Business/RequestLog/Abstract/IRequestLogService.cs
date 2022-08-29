using PriceHunter.Common.Data.Abstract;

namespace PriceHunter.Business.RequestLog.Abstract
{
    public interface IRequestLogService : IService
    {
        Task<bool> SaveAsync(Model.RequestLog.RequestLog entity, CancellationToken cancellationToken);
    }
}