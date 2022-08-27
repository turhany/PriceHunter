using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using PriceHunter.Cache.Abstract;
using PriceHunter.Cache.Constants;

namespace PriceHunter.Cache.Concrete
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        public RedisCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async  Task<T> GetOrSetObjectAsync<T>(string key, Func<T> code, int durationAsMinute = CacheConstants.DefaultCacheDuration)
        {
            if (await ExistObjectAsync<T>(key))
            {
                return await GetObjectAsync<T>(key);
            }

            var result = code.Invoke();

            await SetObjectAsync(key, result, durationAsMinute);
            return result;
        }
        
        public async  Task<T> GetOrSetObjectAsync<T>(string key, Func<Task<T>> code, int durationAsMinute = CacheConstants.DefaultCacheDuration)
        {
            if (await ExistObjectAsync<T>(key))
            {
                return await GetObjectAsync<T>(key);
            }

            var result = await code.Invoke();

            await SetObjectAsync(key, result, durationAsMinute);
            return result;
        }

        public async Task SetObjectAsync<T>(string key, T value, int durationAsMinute = CacheConstants.DefaultCacheDuration)
        {
            key = $"{key}_{Thread.CurrentThread.CurrentUICulture.Name}";
            await _distributedCache.SetStringAsync(key, JsonConvert.SerializeObject(value), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(durationAsMinute)
            });
        }

        public async Task<T> GetObjectAsync<T>(string key)
        {
            key = $"{key}_{Thread.CurrentThread.CurrentUICulture.Name}";
            var value = await _distributedCache.GetStringAsync(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public async Task<bool> ExistObjectAsync<T>(string key)
        {
            key = $"{key}_{Thread.CurrentThread.CurrentUICulture.Name}";
            var value = await _distributedCache.GetStringAsync(key);
            return value != null;
        }

        public async Task RemoveAsync(string key)
        {
            key = $"{key}_{Thread.CurrentThread.CurrentUICulture.Name}";
            await _distributedCache.RemoveAsync(key);
        }
    }
}