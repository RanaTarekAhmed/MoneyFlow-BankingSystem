using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
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

		public AuthenticationProperties GetExternalAuthenticationProperties(string provider, string redirectUrl)
		{
			return _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
		}

		public async Task<ExternalAuthOutcome> HandleExternalLoginCallbackAsync()
		{
			var info = await _signInManager.GetExternalLoginInfoAsync();

			if (info == null)
			{
				return ExternalAuthOutcome.Failed("Google sign-in was cancelled or could not be completed.");
			}

			var signInResult = await _signInManager.ExternalLoginSignInAsync(
				info.LoginProvider,
				info.ProviderKey,
				isPersistent: false,
				bypassTwoFactor: true);

			if (signInResult.IsLockedOut)
			{
				return ExternalAuthOutcome.Failed("This account is locked. Try again later.");
			}

			if (signInResult.Succeeded)
			{
				var signedInUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

				if (signedInUser == null)
				{
					return ExternalAuthOutcome.Failed("Unable to sign in with Google.");
				}

				return ExternalAuthOutcome.Success(signedInUser);
			}

			var email = info.Principal.FindFirstValue(ClaimTypes.Email);

			if (string.IsNullOrWhiteSpace(email))
			{
				return ExternalAuthOutcome.Failed("Google did not provide an email address.");
			}

			var existingUser = await _userManager.FindByEmailAsync(email);

			if (existingUser != null)
			{
				var addLoginResult = await _userManager.AddLoginAsync(existingUser, info);

				if (!addLoginResult.Succeeded)
				{
					return ExternalAuthOutcome.Failed(
						addLoginResult.Errors.FirstOrDefault()?.Description
						?? "Unable to link this Google account.");
				}

				await _signInManager.SignInAsync(existingUser, isPersistent: false);
				return ExternalAuthOutcome.Success(existingUser);
			}

			return ExternalAuthOutcome.RegistrationRequired();
		}

		public async Task<ExternalRegisterVM?> GetExternalRegisterPrefillAsync()
		{
			var info = await _signInManager.GetExternalLoginInfoAsync();

			if (info == null)
			{
				return null;
			}

			var email = info.Principal.FindFirstValue(ClaimTypes.Email);

			if (string.IsNullOrWhiteSpace(email))
			{
				return null;
			}

			var (firstName, lastName) = GetNamesFromPrincipal(info.Principal);

			return new ExternalRegisterVM
			{
				Email = email,
				FirstName = firstName,
				LastName = lastName
			};
		}

		public async Task<(IdentityResult Result, ApplicationUser? User)> CompleteExternalRegisterAsync(ExternalRegisterVM model)
		{
			var info = await _signInManager.GetExternalLoginInfoAsync();

			if (info == null)
			{
				return (IdentityResult.Failed(new IdentityError
				{
					Description = "Google sign-in expired. Please try again."
				}), null);
			}

			var email = info.Principal.FindFirstValue(ClaimTypes.Email);

			if (string.IsNullOrWhiteSpace(email))
			{
				return (IdentityResult.Failed(new IdentityError
				{
					Description = "Google did not provide an email address."
				}), null);
			}

			var user = new ApplicationUser
			{
				UserName = email,
				Email = email,
				EmailConfirmed = true,
				FirstName = model.FirstName,
				LastName = model.LastName,
				DateOfBirth = model.DateOfBirth,
				Address = model.Address
			};

			var createResult = await _userManager.CreateAsync(user);

			if (!createResult.Succeeded)
			{
				return (createResult, null);
			}

			var addLoginResult = await _userManager.AddLoginAsync(user, info);

			if (!addLoginResult.Succeeded)
			{
				return (addLoginResult, null);
			}

			var customer = new Customer(model.NationalId, user.Id);
			await _customerRepository.AddAsync(customer);

			var roleResult = await _userManager.AddToRoleAsync(user, "Customer");

			if (!roleResult.Succeeded)
			{
				return (roleResult, null);
			}

			await _signInManager.SignInAsync(user, isPersistent: false);
			return (IdentityResult.Success, user);
		}

		private static (string FirstName, string LastName) GetNamesFromPrincipal(ClaimsPrincipal principal)
		{
			var firstName = principal.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;
			var lastName = principal.FindFirstValue(ClaimTypes.Surname) ?? string.Empty;

			if (!string.IsNullOrWhiteSpace(firstName) || !string.IsNullOrWhiteSpace(lastName))
			{
				return (firstName, lastName);
			}

			var fullName = principal.FindFirstValue(ClaimTypes.Name);

			if (string.IsNullOrWhiteSpace(fullName))
			{
				return (string.Empty, string.Empty);
			}

			var parts = fullName.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
			firstName = parts[0];
			lastName = parts.Length > 1 ? parts[1] : string.Empty;
			return (firstName, lastName);
		}
	}
}
