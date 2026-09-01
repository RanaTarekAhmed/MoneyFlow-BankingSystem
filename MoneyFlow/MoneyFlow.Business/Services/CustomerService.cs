using Microsoft.AspNetCore.Identity;
using MoneyFlow.Business.Common;
using MoneyFlow.Business.Services.Interfaces;
using MoneyFlow.Business.ViewModels.Customer;
using MoneyFlow.Data.Entities;
using MoneyFlow.Data.Repositories.Interfaces;
using System.Linq.Expressions;


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

        public async Task<PagedResult<CustomerListVM>> GetCustomersPagedAsync( int pageNumber,int pageSize,string? search)
		{
			search = search?.Trim();
            Expression<Func<Customer, bool>>? filter = null;
			if (!string.IsNullOrEmpty(search)){
				filter=c=>c.NationalId.Contains(search) || c.User.FirstName.Contains(search) || c.User.LastName.Contains(search) || c.User.Email != null && c.User.Email.Contains(search)|| (c.User.UserName != null && c.User.UserName.Contains(search)) || (c.User.PhoneNumber != null && c.User.PhoneNumber.Contains(search));
			}
            var (customers, totalCount) = await _customerRepository.GetPagedAsync(pageNumber,pageSize,filter);

            var customerVMs = customers.Select(c => new CustomerListVM {
          Id = c.Id,
          UserName = c.User.UserName ?? string.Empty,
          FirstName = c.User.FirstName,
          LastName = c.User.LastName,
          Email = c.User.Email ?? string.Empty,
          PhoneNumber = c.User.PhoneNumber ?? string.Empty,
          NationalId = c.NationalId,
          CreatedAt = c.CreatedAt,
          IsDeleted = c.IsDeleted,

           AccountsCount = c.Accounts.Count(a => !a.IsDeleted)
            })
      .ToList();

            return new PagedResult<CustomerListVM>
            {
                Items = customerVMs,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

    }
}
