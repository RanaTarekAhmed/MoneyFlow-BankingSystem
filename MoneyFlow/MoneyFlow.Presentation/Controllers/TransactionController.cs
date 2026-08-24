using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.Business.Services;
using MoneyFlow.Business.Services.Interfaces;
using System.Security.Claims;

namespace MoneyFlow.Presentation.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 5;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _transactionService.GetCustomerTransactionsPagedAsync(userId, page, pageSize, null);

            return View(result);
        }

        public async Task<IActionResult> Details()
        {
            return View();
        }
    }
}
