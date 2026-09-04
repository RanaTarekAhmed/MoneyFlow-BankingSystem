using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.Business.Services.Interfaces;
using MoneyFlow.Business.ViewModels.Authentication;
using MoneyFlow.Business.ViewModels.Customer;
using MoneyFlow.Business.ViewModels.Employee;
using System.Security.Claims;


namespace MoneyFlow.Presentation.Controllers
{
 
	[Authorize(Roles = "Employee,Admin")]
	public class EmployeeController : Controller
	{
	   
		private readonly ICustomerService _customerService;
		private readonly IAuthService _authService;
		private readonly IDashboardService _dashboardService;
		private readonly IEmployeeService _employeeService;

        public EmployeeController(ICustomerService customerService, IAuthService authService, IDashboardService dashboardService, IEmployeeService employeeService)
        {
            _customerService = customerService;
            _authService = authService;
            _dashboardService = dashboardService;
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index()
		{
			var dashboard = await _dashboardService.GetEmployeeDashboardAsync(1, 5);
			return View(dashboard);
		}

		public async Task<IActionResult> allCustomers(int page = 1, string? search = null)
		{
			int pageSize = 5;

			var customers = await _customerService.GetCustomersPagedAsync(
				page,
				pageSize,
				search);

			var vm = new CustomerIndexVM
			{
				Customers = customers,
				Search = search
			};

			return View(vm);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RegisterCustomer(RegisterVM model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var result = await _authService.RegisterAsync(model);

			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}

				return BadRequest(ModelState);
			}

			return Ok(new
			{
				success = true,
				message = "Customer registered successfully."
			});
		}

		[HttpGet]
		public async Task<IActionResult> CustomerOverview(int id)
		{
			var customer = await _customerService.GetCustomerOverviewAsync(id);

			if (customer == null)
				return NotFound();

			return View(customer);
		}

		[HttpGet]
		public async Task<IActionResult> Profile()
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			var profileInformation = await _employeeService.GetEmployeeProfileAsync(userId);

			if (profileInformation == null)
			{
				return NotFound();
			}

			var model = new EmployeeProfileVM
			{
				ProfileInformation = profileInformation
			};

			return View(model);
		}
	}
}
