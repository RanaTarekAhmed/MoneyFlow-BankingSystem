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


        public AccountController(
             IAccountService accountService,
             UserManager<ApplicationUser> userManager)
        {
            _accountService = accountService;
            _userManager = userManager;
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
                Query = query
            };

            return View(result);
        }

        [Authorize(Roles = "Employee, Admin")]
        public async Task<IActionResult> EmployeeDetails()
        {
            return View();
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
