using Hangfire;
using PriceHunter.Business.Product.Concrete; 
using PriceHunter.Common.Constans;
using PriceHunter.Common.Data.Abstract;

namespace PriceHunter.ScheduleService.Schedules
{
    public static class RecurringJobs
    {
        public static async Task CheckProductPricesAsync(IServiceScopeFactory serviceScopeFactory)
        {
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                var supplierRepository = serviceScope.ServiceProvider.GetRequiredService<IGenericRepository<PriceHunter.Model.Supplier.Supplier>>();
                var suppliers = supplierRepository.Find(p => p.IsDeleted == false).ToList();

                foreach (var supplier in suppliers)
                {
                    var jobName = $"{nameof(CheckProductPricesAsync)}-{supplier.Name}";
                    var jobCron = string.Format(AppConstants.SupplierPriceControlCronJobTemplate, supplier.PriceControlPeriodAsMinute);

                    RecurringJob.RemoveIfExists(jobName);
                    RecurringJob.AddOrUpdate<ProductService>(jobName, job => job.CheckProductPricesAsync(supplier.Id, CancellationToken.None), jobCron, TimeZoneInfo.Local);
                }
            }
        }
    }
}