using Microsoft.AspNetCore.Identity;
using MoneyFlow.Business.Common;
using MoneyFlow.Business.ViewModels.Customer;


namespace MoneyFlow.Business.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerTopBarVM?> GetCustomerTopBarDataAsync(string? userId);
        Task<ProfileInformationVM?> GetCustomerProfileAsync(string? userId);
        Task<IdentityResult> UpdateCustomerProfileAsync(string? userId, ProfileInformationVM profileInformation);
        Task<IdentityResult> ChangePasswordAsync(string? userId, ChangePasswordVM model);
        Task<PagedResult<CustomerListVM>> GetCustomersPagedAsync(int pageNumber,int pageSize,string? search);
    }
}
