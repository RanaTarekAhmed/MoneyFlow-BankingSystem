using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.Business.Services.Interfaces;
using MoneyFlow.Data.Entities;
using MoneyFlow.Data.Repositories.Interfaces;
using MoneyFlow.Business.ViewModels.Accounts;

namespace MoneyFlow.Presentation.Controllers
{
    [Authorize(Roles = "Customer")]
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
                ModelState.AddModelError(string.Empty, result.Message);

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

            TempData["SuccessMessage"] = result.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}
