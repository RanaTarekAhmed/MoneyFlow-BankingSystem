using Microsoft.AspNetCore.Mvc;

namespace MoneyFlow.Presentation.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Accounts()
        {
            return View();
        }
        public IActionResult AccountDetails()
        {
            return View();
        }
        public IActionResult Customers()
        {
            return View();
        }
        public IActionResult CustomerDetails()
        {
            return View();
        }
        public IActionResult Transactions()
        {
            return View();
        }
        public IActionResult Operations()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }
    }
}
