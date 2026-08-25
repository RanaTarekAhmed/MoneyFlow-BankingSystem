using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.Business.Services.Interfaces;
using MoneyFlow.Business.ViewModels.Authentication;


namespace MoneyFlowSandbox.Controllers
{
	public class AuthController : Controller
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpGet]
		public async Task<IActionResult> Login()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login(LoginVM model)
		{
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
			return RedirectToAction("Index", "Customer");
		}

		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			await _authService.LogoutAsync();

			return RedirectToAction("Login");
		}

		[HttpGet]
		public async Task<IActionResult> Register()
		{
			return View();
		}
	  
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterVM model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var result = await _authService.RegisterAsync(model);

			if (!result.Succeeded)
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
							ModelState.AddModelError(
								nameof(model.Password),
								"Password does not meet the required criteria.");
							break;

						case "DuplicateUserName":
							ModelState.AddModelError(
								nameof(model.Email),
								"This email is already registered.");
							break;

						default:
							ModelState.AddModelError(
								string.Empty,
								error.Description);
							break;
					}
				}

				return View(model);
			}

            TempData["Success"] = "Account created successfully. Please log in.";

            return RedirectToAction("Login");
		}
	}
}
