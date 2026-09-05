using Microsoft.AspNetCore.Identity;
using MoneyFlow.Business.Services.Interfaces;
using MoneyFlow.Business.ViewModels.Employee;
using MoneyFlow.Data.Entities;
using MoneyFlow.Data.Repositories.Interfaces;


namespace MoneyFlow.Business.Services
{
	public class EmployeeService : IEmployeeService
	{
		private readonly IEmployeeRepository _employeeRepository;
		private readonly UserManager<ApplicationUser> _userManager;

		public EmployeeService(UserManager<ApplicationUser> userManager, IEmployeeRepository employeeRepository)
		{
			_userManager = userManager;
			_employeeRepository = employeeRepository;
		}

		public async Task<IdentityResult> ChangePasswordAsync(string? userId, ChangePasswordVM model)
		{
			if (string.IsNullOrEmpty(userId))
			{
                return IdentityResult.Failed(new IdentityError { Description = "User ID is required." });
            }

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }

			return await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        }

		public async Task<ProfileInformationVM?> GetEmployeeProfileAsync(string? userId)
		{
			if (string.IsNullOrEmpty(userId))
			{
				return null;
			}

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return null;
			}
			var employee = await _employeeRepository.GetAsync(e => e.UserId == userId);

			return new ProfileInformationVM
			{
				EmployeeId = employee?.Id,
				Salary = employee?.Salary,
				HireDate = employee?.HireDate,
				CreatedAt = employee?.CreatedAt,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email ?? "",
				Address = user.Address,
				DateOfBirth = user.DateOfBirth
			};
		}

		public async Task<IdentityResult> UpdateEmployeeProfileAsync(string? userId, ProfileInformationVM profileInformation)
		{
			if (string.IsNullOrEmpty(userId))
			{
				return IdentityResult.Failed(new IdentityError { Description = "User ID is required." });
			}

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return IdentityResult.Failed(new IdentityError { Description = "User not found." });
			}

			user.FirstName = profileInformation.FirstName;
			user.LastName = profileInformation.LastName;
			user.Email = profileInformation.Email;
			user.UserName = profileInformation.Email;
			user.Address = profileInformation.Address;
			user.DateOfBirth = profileInformation.DateOfBirth;

			return await _userManager.UpdateAsync(user);
		}
	}
}
