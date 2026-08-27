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

        public CustomerService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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
	}
}
