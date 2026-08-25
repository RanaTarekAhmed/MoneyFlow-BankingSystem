using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.Data.Entities;
using MoneyFlow.Data.Repositories.Interfaces;
using MoneyFlow.Presentation.ModelVM.Accounts;

namespace MoneyFlow.Presentation.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICustomerRepository _customerRepository;

        public AccountController(
     IAccountRepository accountRepository,
     ITransactionRepository transactionRepository,
     ICustomerRepository customerRepository,
     UserManager<ApplicationUser> userManager)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _customerRepository = customerRepository;
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
            var accounts = await _accountRepository.GetAllAsync(a => a.CustomerId == customer.Id );
            var accountVMs = accounts.Select(a => new AccountSummaryVM
            {
                Id = a.Id,
                AccountNumber = a.AccountNumber,
                AccountType = a.AccountType,
                Status = a.Status,
                Balance = a.Balance,
                OpenDate = a.OpenDate
            }).ToList();

            var accountIds = accounts.Select(a => a.Id).ToList();

            var transactions = await _transactionRepository.GetAllAsync(
                t => (t.SenderAccountId.HasValue && accountIds.Contains(t.SenderAccountId.Value))
                  || (t.ReceiverAccountId.HasValue && accountIds.Contains(t.ReceiverAccountId.Value))
            );

            var transactionVMs = transactions.Select(t => new TransactionVM
            {
                Id = t.Id,
                TransactionNumber = t.TransactionNumber,
                TransactionType = t.TransactionType,
                Amount = t.Amount,
                Status = t.Status,
                TransactionDate = t.TransactionDate,
                Description = t.Description,

                SenderAccountId = t.SenderAccountId,
                ReceiverAccountId = t.ReceiverAccountId
            }).ToList();

            var model = new MyAccountsVM
            {
                Accounts = accountVMs,
                Transactions = transactionVMs
            };

            return View(model);
        }


        public async Task<IActionResult> AccountDetails(int id)
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

            var account = await _accountRepository.GetAsync(
                a => a.Id == id && a.CustomerId == customer.Id
            );

            if (account == null)
            {
                return NotFound();
            }

            var transactions = await _transactionRepository.GetAllAsync(
                t => t.SenderAccountId == id || t.ReceiverAccountId == id
            );

            var vm = new AccountDetailsVM
            {
                Account = new AccountSummaryVM
                {
                    Id = account.Id,
                    AccountNumber = account.AccountNumber,
                    AccountType = account.AccountType,
                    Status = account.Status,
                    Balance = account.Balance,
                    OpenDate = account.OpenDate
                },

                Transactions = transactions.Select(t => new TransactionVM
                {
                    Id = t.Id,
                    TransactionNumber = t.TransactionNumber,
                    TransactionType = t.TransactionType,
                    Amount = t.Amount,
                    Status = t.Status,
                    TransactionDate = t.TransactionDate,
                    Description = t.Description,
                    SenderAccountId = t.SenderAccountId,
                    ReceiverAccountId = t.ReceiverAccountId
                }).ToList()
            };

            return View(vm);
        }
    }
}
