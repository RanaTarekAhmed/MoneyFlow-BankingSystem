using MoneyFlow.Business.ViewModels.Customer;


namespace MoneyFlow.Business.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerTopBarVM?> GetCustomerTopBarDataAsync(string? userId);
    }
}
