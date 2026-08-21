using Microsoft.AspNetCore.Identity;
using MoneyFlow.Business.Services.Interfaces;
using MoneyFlow.Business.ViewModels.Authentication;
using MoneyFlow.Data.Entities;
using MoneyFlow.Data.Repositories.Interfaces;


namespace MoneyFlow.Business.Services
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ICustomerRepository _customerRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(UserManager<ApplicationUser> userManager, ICustomerRepository customerRepository , SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_customerRepository = customerRepository;
			_signInManager = signInManager;
		}

		public async Task<SignInResult> LoginAsync(LoginVM model)
		{
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return SignInResult.Failed;

            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);
            return result;
        }

		public async Task LogoutAsync()
		{
            await _signInManager.SignOutAsync();
        }

		public async Task<IdentityResult> RegisterAsync(RegisterVM model)
		{
			var user = new ApplicationUser
			{
				UserName = model.Email,
				FirstName = model.FirstName,
				LastName = model.LastName,
				DateOfBirth = model.DateOfBirth,
				Email = model.Email,
				Address = model.Address
			};

			var result = await _userManager.CreateAsync(user, model.Password);

			if (!result.Succeeded)
			{
				return result;
			}

			var customer = new Customer(model.NationalId, user.Id);
			await _customerRepository.AddAsync(customer);

			var resultRole = await _userManager.AddToRoleAsync(user, "Customer");

			if (!resultRole.Succeeded)
			{
				return resultRole;
			}

			return IdentityResult.Success;
		}
	}
}
