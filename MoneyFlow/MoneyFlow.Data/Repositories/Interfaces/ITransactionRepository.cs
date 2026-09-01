using System.Linq.Expressions;
using MoneyFlow.Data.Entities;
using MoneyFlow.Data.Enums;


namespace MoneyFlow.Data.Repositories.Interfaces
{
	public interface ITransactionRepository
	{
		Task<List<Transaction>> GetAllAsync(Expression<Func<Transaction, bool>>? filter);
		Task<Transaction?> GetAsync(Expression<Func<Transaction, bool>> filter);
		Task<(List<Transaction> Items, int TotalCount)> GetCustomerTransactionsPagedAsync
			(
			int customerId, 
			int pageNumber, 
			int pageSize,
            Expression<Func<Transaction, bool>>? filter
            );
		Task<(List<Transaction> Items, int TotalCount)> GetAllTransactionsPagedAsync
            (
            int pageNumber,
            int pageSize,
            Expression<Func<Transaction, bool>>? filter
            );
        Task<Transaction?> GetCustomerTransactionByIdAsync(int transactionId, int customerId);
		Task AddAsync(Transaction transaction);
		Task UpdateStatusAsync(int id, TransactionStatus status);
	}
}
