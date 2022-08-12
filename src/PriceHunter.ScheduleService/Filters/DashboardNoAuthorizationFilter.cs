using Hangfire.Dashboard;

namespace PriceHunter.ScheduleService.Filters
{
    public class DashboardNoAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext dashboardContext)
        {
            return true;
        }
    }

}