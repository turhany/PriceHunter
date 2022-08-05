using System;
using System.Threading.Tasks;

namespace PriceHunter.Common.Lock.Abstract
{
    public interface ILockService
    {
        Task<IDisposable> CreateLockAsync(string key);
    }
}