namespace PriceHunter.Lock.Abstract
{
    public interface ILockService
    {
        Task<IDisposable> CreateLockAsync(string key, CancellationToken cancellationToken);
    }
}