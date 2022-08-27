using PriceHunter.Cache.Constants;

namespace PriceHunter.Cache.Abstract
{
    public interface ICacheService
    {
        Task<T> GetOrSetObjectAsync<T>(string key, Func<T> code, int durationAsMinute = CacheConstants.DefaultCacheDuration);
        Task<T> GetOrSetObjectAsync<T>(string key, Func<Task<T>> code, int durationAsMinute = CacheConstants.DefaultCacheDuration);
        Task SetObjectAsync<T>(string key, T value, int durationAsMinute = CacheConstants.DefaultCacheDuration);
        Task<T> GetObjectAsync<T>(string key);
        Task<bool> ExistObjectAsync<T>(string key);

        Task RemoveAsync(string key);
    }
}