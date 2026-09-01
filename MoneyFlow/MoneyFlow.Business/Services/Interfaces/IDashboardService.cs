using MoneyFlow.Business.ViewModels.Customer;
using MoneyFlow.Business.ViewModels;

namespace MoneyFlow.Business.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardVM?> GetDashboardAsync(string userId);
        Task<EmployeeDashboardVM> GetEmployeeDashboardAsync(int pageNumber, int pageSize);
    }
}