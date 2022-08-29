using PriceHunter.Business.RequestLog.Abstract;
using PriceHunter.Common.Data.Abstract;

namespace PriceHunter.Business.RequestLog.Concrete
{
    public class RequestLogService : IRequestLogService
    {
        private readonly IGenericRepository<Model.RequestLog.RequestLog> _requestLogRepository;

        public RequestLogService(IGenericRepository<Model.RequestLog.RequestLog> requestLogRepository)
        {
            _requestLogRepository = requestLogRepository;
        }

        /// <summary>
        /// Save request log
        /// </summary>
        /// <param name="entity">RequestLog Item</param>
        /// <returns>bool</returns>
        public async Task<bool> SaveAsync(Model.RequestLog.RequestLog entity, CancellationToken cancellationToken)
        {
            return await _requestLogRepository.InsertAsync(entity, cancellationToken) != null;
        }
    }
}