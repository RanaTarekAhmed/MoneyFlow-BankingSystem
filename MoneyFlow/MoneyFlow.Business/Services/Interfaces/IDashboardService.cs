using MoneyFlow.Business.ViewModels.Customer;

namespace MoneyFlow.Business.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardVM?> GetDashboardAsync(string userId);
    }
}