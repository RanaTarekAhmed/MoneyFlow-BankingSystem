using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.Business.Services.Interfaces;
using MoneyFlow.Data.Entities;
using MoneyFlow.Data.Repositories.Interfaces;
using MoneyFlow.Business.ViewModels.Accounts;

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
                var transferModel = await _accountService.GetTransferModelAsync(user.Id);

                if (transferModel == null)
                {
                    return NotFound();
                }

                transferModel.SenderAccountId = model.SenderAccountId;

                transferModel.ReceiverAccountNumber = model.ReceiverAccountNumber;

                transferModel.Amount = model.Amount;

                transferModel.Description = model.Description;

                return View(transferModel);
            }

            var result = await _accountService.TransferAsync(user.Id, model);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty,result.Message);

                var transferModel = await _accountService.GetTransferModelAsync(user.Id);

                if (transferModel == null)
                {
                    return NotFound();
                }

                transferModel.SenderAccountId = model.SenderAccountId;

                transferModel.ReceiverAccountNumber = model.ReceiverAccountNumber;

                transferModel.Amount = model.Amount;

                transferModel.Description = model.Description;

                return View(transferModel);
            }

            ViewBag.TransferSuccessful = true;

            ViewBag.TransactionNumber = result.Transaction?.TransactionNumber;

            ViewBag.TransactionDate = result.Transaction?.TransactionDate;


            return View(model);
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
