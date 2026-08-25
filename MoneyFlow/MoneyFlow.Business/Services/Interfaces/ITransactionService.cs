using MoneyFlow.Business.Common;
using MoneyFlow.Business.ViewModels.Transaction;
using System.Linq.Expressions;
using MoneyFlow.Data.Entities;

namespace MoneyFlow.Business.Services.Interfaces
{
	public interface ITransactionService
	{
		Task<PagedResult<TransactionVM>> GetCustomerTransactionsPagedAsync
			(
			string? userId,
			int pageNumber,
			int pageSize,
            TransactionQueryVM? query
            );
		Task<TransactionDetailsVM?> GetCustomerTransactionByIdAsync(int transactionId, string? userId);
    }
}
