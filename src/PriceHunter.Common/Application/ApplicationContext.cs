using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace PriceHunter.Common.Application
{
  public class ApplicationContext
    {
        private static ApplicationContext _instance;
        public static ApplicationContext Instance => _instance ??= new ApplicationContext();
        
        public static IHttpContextAccessor Context { get; set; }
          
        private ApplicationContext()
        {
            
        }
        
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            Context = httpContextAccessor;
        }
          
        public static void ConfigureThreadPool(IConfiguration configuration)
        {
            var workerThreads = Convert.ToInt32(configuration["ThreadPool:WorkerThreads"]);
            var completionPortThreads = Convert.ToInt32(configuration["ThreadPool:CompletionPortThreads"]);

            System.Threading.ThreadPool.GetMaxThreads(out var maxWorkerThreads, out var maxCompletionPortThreads);
            System.Threading.ThreadPool.GetMinThreads(out var minWorkerThreads, out var minCompletionPortThreads);

            workerThreads = Math.Max(workerThreads, minWorkerThreads);
            workerThreads = Math.Min(workerThreads, maxWorkerThreads);
            completionPortThreads = Math.Max(completionPortThreads, minCompletionPortThreads);
            completionPortThreads = Math.Min(completionPortThreads, maxCompletionPortThreads);

            System.Threading.ThreadPool.SetMinThreads(workerThreads, completionPortThreads);
        } 
    }
}