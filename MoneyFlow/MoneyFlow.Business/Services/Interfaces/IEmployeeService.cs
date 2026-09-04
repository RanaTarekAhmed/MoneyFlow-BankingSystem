using Microsoft.AspNetCore.Identity;
using MoneyFlow.Business.ViewModels.Employee;


namespace MoneyFlow.Business.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<ProfileInformationVM?> GetEmployeeProfileAsync(string? userId);
        Task<IdentityResult> UpdateEmployeeProfileAsync(string? userId, ProfileInformationVM profileInformation);
        Task<IdentityResult> ChangePasswordAsync(string? userId, ChangePasswordVM model);
    }
}
