using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.Business.Services.Interfaces;
using MoneyFlow.Business.ViewModels.Accounts;
using MoneyFlow.Data.Entities;
using MoneyFlow.Data.Repositories;
using MoneyFlow.Data.Repositories.Interfaces;

namespace MoneyFlow.Presentation.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICustomerService _customerService;

        public AccountController(
             IAccountService accountService,
             UserManager<ApplicationUser> userManager,
             ICustomerService customerService)
        {
            _accountService = accountService;
            _userManager = userManager;
            _customerService = customerService;
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var model = await _accountService.GetMyAccountsAsync(user.Id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AccountDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var model = await _accountService.GetAccountDetailsAsync(
                user.Id,
                id
            );

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<IActionResult> Transfer()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var model = await _accountService.GetTransferModelAsync(
                user.Id
            );

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Transfer(TransferVM model)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return View(await RepopulateTransferModelAsync(user.Id, model));
            }

            var result = await _accountService.TransferAsync(user.Id, model);

            if (!result.Success)
            {
                if (!string.IsNullOrEmpty(result.Message) &&
                    (result.Message.Contains("receiver", StringComparison.OrdinalIgnoreCase) ||
                     result.Message.Contains("recipient", StringComparison.OrdinalIgnoreCase) ||
                     result.Message.Contains("exist", StringComparison.OrdinalIgnoreCase)))
                {
                    ModelState.AddModelError(nameof(model.ReceiverAccountNumber), result.Message);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Message ?? "An unexpected error occurred.");
                }

                return View(await RepopulateTransferModelAsync(user.Id, model));
            }

            ViewBag.TransferSuccessful = true;
            ViewBag.TransactionNumber = result.Transaction?.TransactionNumber;
            ViewBag.TransactionDate = result.Transaction?.TransactionDate;

            return View(model);
        }

        private async Task<TransferVM> RepopulateTransferModelAsync(string userId, TransferVM submittedModel)
        {
            var transferModel = await _accountService.GetTransferModelAsync(userId);
            if (transferModel != null)
            {
                transferModel.SenderAccountId = submittedModel.SenderAccountId;
                transferModel.ReceiverAccountNumber = submittedModel.ReceiverAccountNumber;
                transferModel.Amount = submittedModel.Amount;
                transferModel.Description = submittedModel.Description;
                return transferModel;
            }

            return submittedModel;
        }

        [Authorize(Roles = "Employee, Admin")]
        public async Task<IActionResult> EmployeeIndex(int page = 1, AccountQueryVM? query = null)
        {
            int pageSize = 5;

            var accounts = await _accountService.GetAllAccountsPagedAsync(page, pageSize, query);

            var summary = await _accountService.GetAllAccountsSummaryAsync();

            var result = new EmployeeAccountIndexVM
            {
                Summary = summary,
                Accounts = accounts,
                Query = query,
                OpenAccount = new OpenAccountVM()
            };

            return View(result);
        }

        [Authorize(Roles = "Employee, Admin")]
        [HttpGet]
        public async Task<IActionResult> SearchCustomers(string? query)
        {
            var pagedCustomers = await _customerService.GetCustomersPagedAsync(1, 10, query);
            var results = pagedCustomers.Items.Select(c => new
            {
                id = c.Id,
                name = $"{c.FirstName} {c.LastName}".Trim(),
                email = c.Email,
                nationalId = c.NationalId
            });

            return Json(results);
        }

        [Authorize(Roles = "Employee, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Open(OpenAccountVM model, int page = 1, AccountQueryVM? query = null)
        {
            if (model.CustomerId <= 0)
            {
                ModelState.AddModelError(nameof(model.CustomerId), "Please select a valid customer.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ShowOpenAccountModal = true;
                int pageSize = 5;
                var accounts = await _accountService.GetAllAccountsPagedAsync(page, pageSize, query);
                var summary = await _accountService.GetAllAccountsSummaryAsync();

                var indexModel = new EmployeeAccountIndexVM
                {
                    Summary = summary,
                    Accounts = accounts,
                    Query = query,
                    OpenAccount = model
                };

                return View("EmployeeIndex", indexModel);
            }

            var result = await _accountService.OpenAccountAsync(model);

            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Failed to open account. Please check customer status and try again.");
                ViewBag.ShowOpenAccountModal = true;
                int pageSize = 5;
                var accounts = await _accountService.GetAllAccountsPagedAsync(page, pageSize, query);
                var summary = await _accountService.GetAllAccountsSummaryAsync();

                var indexModel = new EmployeeAccountIndexVM
                {
                    Summary = summary,
                    Accounts = accounts,
                    Query = query,
                    OpenAccount = model
                };

                return View("EmployeeIndex", indexModel);
            }

            TempData["SuccessMessage"] = $"New {model.AccountType} account created successfully.";
            return RedirectToAction("EmployeeIndex");
        }

        [Authorize(Roles = "Employee, Admin")]
        [HttpGet]
        public async Task<IActionResult> EmployeeDetails(int id)
        {
            var result = await _accountService.GetAccountDetailsAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }


        [Authorize(Roles = "Employee, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(UpdateStatusVM model, int page = 1, AccountQueryVM? query = null)
        {
            if (model.AccountId <= 0)
            {
                ModelState.AddModelError(nameof(model.AccountId), "Invalid account selected.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ShowUpdateStatusModal = true;
                int pageSize = 5;
                var accounts = await _accountService.GetAllAccountsPagedAsync(page, pageSize, query);
                var summary = await _accountService.GetAllAccountsSummaryAsync();

                var indexModel = new EmployeeAccountIndexVM
                {
                    Summary = summary,
                    Accounts = accounts,
                    Query = query,
                    UpdateStatus = model
                };

                return View("EmployeeIndex", indexModel);
            }

            var result = await _accountService.UpdateStatusAsync(model);

            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Failed to update account status. Account not found.");
                ViewBag.ShowUpdateStatusModal = true;
                int pageSize = 5;
                var accounts = await _accountService.GetAllAccountsPagedAsync(page, pageSize, query);
                var summary = await _accountService.GetAllAccountsSummaryAsync();

                var indexModel = new EmployeeAccountIndexVM
                {
                    Summary = summary,
                    Accounts = accounts,
                    Query = query,
                    UpdateStatus = model
                };

                return View("EmployeeIndex", indexModel);
            }

            TempData["SuccessMessage"] = $"Account {model.AccountNumber ?? ""} status updated to {model.Status} successfully.";
            return RedirectToAction("EmployeeIndex");
        }

        [Authorize(Roles = "Employee, Admin")]
        [HttpGet]
        public IActionResult Operations()
        {
            return View("~/Views/Employee/Operations.cshtml", new CashOperationVM());
        }

        [Authorize(Roles = "Employee, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Operations(CashOperationVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Employee/Operations.cshtml", model);
            }

            var result = model.OperationType.Equals("Withdraw", StringComparison.OrdinalIgnoreCase)
                ? await _accountService.WithdrawAsync(model)
                : await _accountService.DepositAsync(model);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View("~/Views/Employee/Operations.cshtml", model);
            }

            TempData["SuccessMessage"] = result.Message;
            TempData["TransactionNumber"] = result.Transaction?.TransactionNumber;
            TempData["OperationType"] = model.OperationType;
            TempData["AccountNumber"] = model.AccountNumber;
            TempData["Amount"] = model.Amount.ToString("N2");
            TempData["TransactionDate"] = (result.Transaction?.TransactionDate ?? DateTime.Now).ToString("g");

            return RedirectToAction(nameof(Operations));
        }

        [Authorize(Roles = "Employee, Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAccountLookup(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                return BadRequest(new { success = false, message = "Please enter an account number." });
            }

            var account = await _accountService.GetAccountByNumberAsync(accountNumber);

            if (account == null)
            {
                return NotFound(new { success = false, message = "Account number not found." });
            }

            string initials = "NA";
            if (!string.IsNullOrWhiteSpace(account.Account.AccountNumber))
            {
                var parts = account.Account.AccountNumber.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                initials = parts.Length > 1
                    ? $"{parts[0][0]}{parts[^1][0]}".ToUpper()
                    : $"{parts[0][0]}".ToUpper();
            }

            return Json(new
            {
                success = true,
                accountType = account.Account.AccountType,
                accountNumber = account.Account.AccountNumber,
                balance = account.Account.Balance.ToString("C2"),
                status = account.Account.Status.ToString(),
                initials = initials
            });
        }
    }
}
