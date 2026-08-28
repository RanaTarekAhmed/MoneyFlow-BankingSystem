using Microsoft.AspNetCore.Identity;
using MoneyFlow.Business.Services.Interfaces;
using MoneyFlow.Business.ViewModels.Customer;
using MoneyFlow.Data.Entities;
using MoneyFlow.Data.Repositories.Interfaces;


namespace MoneyFlow.Business.Services
{
	public class CustomerService : ICustomerService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ICustomerRepository _customerRepository;

		public CustomerService(UserManager<ApplicationUser> userManager, ICustomerRepository customerRepository)
		{
			_userManager = userManager;
			_customerRepository = customerRepository;
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

		public async Task<ProfileInformationVM?> GetCustomerProfileAsync(string? userId)
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

			var customer = await _customerRepository.GetAsync(c => c.UserId == userId);
			if (customer == null)
			{
				return null;
			}

			return new ProfileInformationVM
			{
				CustomerId = customer.Id,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email ?? "",
				Address = user.Address,
				DateOfBirth = user.DateOfBirth,
				NationalId = customer.NationalId,
				CreatedAt = customer.CreatedAt
			};
		}

		public async Task<CustomerTopBarVM?> GetCustomerTopBarDataAsync(string? userId)
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

			return new CustomerTopBarVM
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email ?? ""
			};
		}

		public async Task<IdentityResult> UpdateCustomerProfileAsync(string? userId, ProfileInformationVM profileInformation)
		{
			if (string.IsNullOrEmpty(userId))
			{
                return IdentityResult.Failed(new IdentityError { Description = "User ID is required." });
            }

            var customer = await _customerRepository.GetAsync(c => c.UserId == userId);
			if (customer == null)
			{
                return IdentityResult.Failed(new IdentityError { Description = "Customer not found." });
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
