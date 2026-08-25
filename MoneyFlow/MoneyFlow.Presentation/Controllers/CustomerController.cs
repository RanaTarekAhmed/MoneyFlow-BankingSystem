using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.Data.Entities;
using MoneyFlow.Data.Enums;
using MoneyFlow.Data.Repositories.Interfaces;

namespace MoneyFlowSandbox.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomerController(ICustomerRepository customerRepository, IAccountRepository accountRepository, ITransactionRepository transactionRepository, UserManager<ApplicationUser> userManager)
        {
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var customer = await _customerRepository.GetAsync(c => c.UserId == user.Id);

            if (customer == null)
            {
                return NotFound();
            }
            
            var accounts = await _accountRepository.GetAllAsync(a => a.CustomerId == customer.Id && a.Status != AccountStatus.Closed);

            var accountIds = accounts.Select(a => a.Id).ToList();
            
            var transactions = accountIds.Count == 0
                ? new List<MoneyFlow.Data.Entities.Transaction>()
                : await _transactionRepository.GetAllAsync(
                    t => (t.SenderAccountId.HasValue && accountIds.Contains(t.SenderAccountId.Value))
                      || (t.ReceiverAccountId.HasValue && accountIds.Contains(t.ReceiverAccountId.Value)));

            var recentTransactions = transactions.OrderByDescending(t => t.TransactionDate).Take(4).ToList();

            var weekAgo = DateTime.UtcNow.AddDays(-7);
            var recentCount = transactions.Count(t => t.TransactionDate >= weekAgo);

            ViewBag.CustomerName = string.IsNullOrWhiteSpace(user.FirstName) ? user.UserName ?? "Customer" : user.FirstName;

            ViewBag.TotalBalance = accounts.Where(a => a.Status == AccountStatus.Active).Sum(a => a.Balance);

            ViewBag.ActiveAccounts = accounts.Count(a => a.Status == AccountStatus.Active);
            ViewBag.RecentTransactionsCount = recentCount;
            ViewBag.RecentTransactions = recentTransactions;
            ViewBag.Accounts = accounts;

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
