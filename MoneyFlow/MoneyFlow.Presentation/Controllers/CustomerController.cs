using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.Business.Services.Interfaces;

namespace MoneyFlowSandbox.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public CustomerController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

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

        public IActionResult Accounts()
        {
            return View();
        }

        public IActionResult AccountDetails()
        {
            return View();
        }

        public IActionResult Transfer()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Notifications()
        {
            return View();
        }

        public IActionResult Settings()
        {
            return View();
        }
    }
}