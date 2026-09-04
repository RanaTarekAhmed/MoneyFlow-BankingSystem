using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.Business.Services.Interfaces;
using MoneyFlow.Business.ViewModels.Authentication;
using MoneyFlow.Data.Entities;


namespace MoneyFlowSandbox.Controllers
{
	public class AuthController : Controller
	{
		private readonly IAuthService _authService;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IAuthenticationSchemeProvider _schemeProvider;

		public AuthController(
			IAuthService authService,
			UserManager<ApplicationUser> userManager,
			IAuthenticationSchemeProvider schemeProvider)
		{
			_authService = authService;
			_userManager = userManager;
			_schemeProvider = schemeProvider;
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Login()
		{
			await SetGoogleAuthFlagAsync();
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login(LoginVM model)
		{
			await SetGoogleAuthFlagAsync();

			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var result = await _authService.LoginAsync(model);
			if (!result.Succeeded)
			{
				ModelState.AddModelError(string.Empty, "Invalid email or password");
				return View(model);
			}
			var user = await _userManager.FindByEmailAsync(model.Email);

			if (user != null && await _userManager.IsInRoleAsync(user, "Employee"))
			{
				return RedirectToAction("Index", "Employee");
			}

			if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
			{
				return RedirectToAction("Index", "Employee");
			}

			return RedirectToAction("Index", "Customer");
		}

		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			await _authService.LogoutAsync();

			return RedirectToAction("Login");
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Register()
		{
			await SetGoogleAuthFlagAsync();
			return View();
		}
	  
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterVM model)
		{
			await SetGoogleAuthFlagAsync();

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var result = await _authService.RegisterAsync(model);

			if (!result.Succeeded)
			{
				AddIdentityErrors(result, nameof(model.Email), nameof(model.Password));
				return View(model);
			}

            TempData["Success"] = "Account created successfully. Please log in.";

            return RedirectToAction("Login");
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ExternalLogin(string provider)
		{
			if (!await IsGoogleEnabledAsync() || !string.Equals(provider, "Google", StringComparison.Ordinal))
			{
				TempData["Error"] = "Google sign-in is not configured.";
				return RedirectToAction(nameof(Login));
			}

			var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth");
			var properties = _authService.GetExternalAuthenticationProperties(provider, redirectUrl!);
			return Challenge(properties, provider);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ExternalLoginCallback(string? remoteError = null)
		{
			if (!string.IsNullOrEmpty(remoteError))
			{
				TempData["Error"] = $"Google sign-in failed: {remoteError}";
				return RedirectToAction(nameof(Login));
			}

			var outcome = await _authService.HandleExternalLoginCallbackAsync();

			if (outcome.RequiresRegistration)
			{
				return RedirectToAction(nameof(ExternalRegister));
			}

			if (!outcome.Succeeded || outcome.User == null)
			{
				TempData["Error"] = outcome.ErrorMessage ?? "Google sign-in failed.";
				return RedirectToAction(nameof(Login));
			}

			return await RedirectSignedInUserAsync(outcome.User);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ExternalRegister()
		{
			var prefill = await _authService.GetExternalRegisterPrefillAsync();

			if (prefill == null)
			{
				TempData["Error"] = "Google sign-in expired. Please try again.";
				return RedirectToAction(nameof(Login));
			}

			return View(prefill);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ExternalRegister(ExternalRegisterVM model)
		{
			var prefill = await _authService.GetExternalRegisterPrefillAsync();

			if (prefill == null)
			{
				TempData["Error"] = "Google sign-in expired. Please try again.";
				return RedirectToAction(nameof(Login));
			}

			model.Email = prefill.Email;
			ModelState.Remove(nameof(model.Email));

			if (!TryValidateModel(model))
			{
				return View(model);
			}

			var (result, user) = await _authService.CompleteExternalRegisterAsync(model);

			if (!result.Succeeded || user == null)
			{
				AddIdentityErrors(result, nameof(model.Email), string.Empty);
				return View(model);
			}

			return await RedirectSignedInUserAsync(user);
		}

		private async Task<IActionResult> RedirectSignedInUserAsync(ApplicationUser user)
		{
			if (await _userManager.IsInRoleAsync(user, "Employee") ||
				await _userManager.IsInRoleAsync(user, "Admin"))
			{
				return RedirectToAction("Index", "Employee");
			}

			return RedirectToAction("Index", "Customer");
		}

		private async Task SetGoogleAuthFlagAsync()
		{
			ViewBag.GoogleAuthEnabled = await IsGoogleEnabledAsync();
		}

		private async Task<bool> IsGoogleEnabledAsync()
		{
			var scheme = await _schemeProvider.GetSchemeAsync("Google");
			return scheme != null;
		}

		private void AddIdentityErrors(IdentityResult result, string emailField, string passwordField)
		{
			foreach (var error in result.Errors)
			{
				switch (error.Code)
				{
					case "PasswordTooShort":
					case "PasswordRequiresDigit":
					case "PasswordRequiresUpper":
					case "PasswordRequiresLower":
					case "PasswordRequiresNonAlphanumeric":
						if (!string.IsNullOrEmpty(passwordField))
						{
							ModelState.AddModelError(
								passwordField,
								"Password does not meet the required criteria.");
						}
						else
						{
							ModelState.AddModelError(string.Empty, error.Description);
						}
						break;

					case "DuplicateUserName":
						ModelState.AddModelError(
							emailField,
							"This email is already registered.");
						break;

					default:
						ModelState.AddModelError(
							string.Empty,
							error.Description);
						break;
				}
			}
		}
	}
}
