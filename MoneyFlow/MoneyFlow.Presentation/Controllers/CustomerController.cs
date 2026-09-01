using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.Business.Services.Interfaces;
using MoneyFlow.Business.ViewModels.Customer;
using System.Security.Claims;

namespace MoneyFlowSandbox.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly ICustomerService _customerService;

        public CustomerController(IDashboardService dashboardService, ICustomerService customerService)
        {
            _dashboardService = dashboardService;
            _customerService = customerService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var model = await _dashboardService.GetDashboardAsync(userId);

            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var profileInformation = await _customerService.GetCustomerProfileAsync(userId);

            if (profileInformation == null)
            {
                return NotFound();
            }

            var model = new CustomerProfileVM
            {
                ProfileInformation = profileInformation
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(CustomerProfileVM model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Clear any ChangePassword validation errors as this post is only for ProfileInformation
            foreach (var key in ModelState.Keys.Where(k => k.StartsWith("ChangePassword")).ToList())
            {
                ModelState.Remove(key);
            }

            if (!ModelState.IsValid)
            {
                var currentProfile = await _customerService.GetCustomerProfileAsync(userId);
                if (currentProfile != null)
                {
                    model.ProfileInformation.NationalId = currentProfile.NationalId;
                    model.ProfileInformation.CustomerId = currentProfile.CustomerId;
                    model.ProfileInformation.CreatedAt = currentProfile.CreatedAt;
                }
                return View(model);
            }

            var result = await _customerService.UpdateCustomerProfileAsync(userId, model.ProfileInformation);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    switch (error.Code)
                    {
                        case "DuplicateEmail":
                        case "DuplicateUserName":
                            ModelState.AddModelError(
                                "ProfileInformation.Email",
                                "This email is already in use.");
                            break;

                        default:
                            ModelState.AddModelError(
                                string.Empty,
                                error.Description);
                            break;
                    }
                }

                var currentProfile = await _customerService.GetCustomerProfileAsync(userId);
                if (currentProfile != null)
                {
                    model.ProfileInformation.NationalId = currentProfile.NationalId;
                    model.ProfileInformation.CustomerId = currentProfile.CustomerId;
                    model.ProfileInformation.CreatedAt = currentProfile.CreatedAt;
                }

                return View(model);
            }

            TempData["Success"] = "Your personal information has been successfully updated.";

            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(CustomerProfileVM model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Clear any ProfileInformation validation errors as this post is only for ChangePassword
            foreach (var key in ModelState.Keys.Where(k => k.StartsWith("ProfileInformation")).ToList())
            {
                ModelState.Remove(key);
            }

            if (!ModelState.IsValid)
            {
                var currentProfile = await _customerService.GetCustomerProfileAsync(userId);

                if (currentProfile != null)
                {
                    model.ProfileInformation.CustomerId = currentProfile.CustomerId;
                    model.ProfileInformation.FirstName = currentProfile.FirstName;
                    model.ProfileInformation.LastName = currentProfile.LastName;
                    model.ProfileInformation.Email = currentProfile.Email;
                    model.ProfileInformation.Address = currentProfile.Address;
                    model.ProfileInformation.DateOfBirth = currentProfile.DateOfBirth;
                    model.ProfileInformation.NationalId = currentProfile.NationalId;
                    model.ProfileInformation.CreatedAt = currentProfile.CreatedAt;
                }

                return View("Profile", model);
            }

            var result = await _customerService.ChangePasswordAsync(userId, model.ChangePassword);

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
                                "ChangePassword.NewPassword",
                                "Password does not meet the required criteria.");
                            break;

                        default:
                            ModelState.AddModelError(
                                "ChangePassword.CurrentPassword",
                                error.Description);
                            break;
                    }
                }

                var currentProfile = await _customerService.GetCustomerProfileAsync(userId);

                if (currentProfile != null)
                {
                    model.ProfileInformation.CustomerId = currentProfile.CustomerId;
                    model.ProfileInformation.FirstName = currentProfile.FirstName;
                    model.ProfileInformation.LastName = currentProfile.LastName;
                    model.ProfileInformation.Email = currentProfile.Email;
                    model.ProfileInformation.Address = currentProfile.Address;
                    model.ProfileInformation.DateOfBirth = currentProfile.DateOfBirth;
                    model.ProfileInformation.NationalId = currentProfile.NationalId;
                    model.ProfileInformation.CreatedAt = currentProfile.CreatedAt;
                }

                return View("Profile", model);
            }

            TempData["Success"] = "Your password has been changed successfully.";

            return RedirectToAction("Profile");
        }

        public IActionResult Settings()
        {
            return View();
        }



        
    }
}