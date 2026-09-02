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
    }
}
