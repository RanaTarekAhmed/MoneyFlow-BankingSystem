using Microsoft.AspNetCore.Mvc;

namespace MoneyFlowSandbox.Controllers
{
    public class CustomerController : Controller
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

        public IActionResult Transfer()
        {
            return View();
        }

        public IActionResult Transactions()
        {
            return View();
        }

        public IActionResult TransactionDetails()
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
