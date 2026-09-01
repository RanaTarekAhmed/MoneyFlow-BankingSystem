using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.Business.Services;
using MoneyFlow.Business.Services.Interfaces;
using MoneyFlow.Business.ViewModels.Transaction;
using System.Security.Claims;

namespace MoneyFlow.Presentation.Controllers
{
    [Authorize(Roles = "Customer")]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> CustomerIndex(int page = 1, TransactionQueryVM? query = null)
        {
            int pageSize = 5;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var transactions = await _transactionService.GetCustomerTransactionsPagedAsync(userId, page, pageSize, query);

            var result = new TransactionIndexVM
            {
                Transactions = transactions,
                Query = query
            };

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> CustomerDetails(int id, int pageNumber)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _transactionService.GetCustomerTransactionByIdAsync(id, userId);

            ViewBag.PageNumber = pageNumber;

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> EmployeeIndex(int page = 1, TransactionQueryVM? query = null)
        {
            int pageSize = 5;

            var transactions = await _transactionService.GetAllTransactionsPagedAsync(page, pageSize, query);

            var result = new EmployeeTransactionIndexVM
            {
                Transactions = transactions,
                Query = query
            };

            return View(result);
        }
    }
}
