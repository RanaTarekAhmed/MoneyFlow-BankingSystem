using MoneyFlow.Data.Entities;
using MoneyFlow.Data.Enums;
using MoneyFlow.Data.Repositories.Interfaces;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MoneyFlow.Data.Database;
using System.Numerics;
using System.Runtime.CompilerServices;



namespace MoneyFlow.Data.Repositories
{
	public class TransactionRepository : ITransactionRepository
	{
		private readonly MoneyFlowDbContext _context;

		public TransactionRepository(MoneyFlowDbContext context)
		{
			_context = context;
		}
		public async Task AddAsync(Transaction transaction)
		{

			await _context.Transactions.AddAsync(transaction);
			await _context.SaveChangesAsync();

		}

		public async Task<List<Transaction>> GetAllAsync(Expression<Func<Transaction, bool>>? filter)
		{
			var query = _context.Transactions.Where(t => (t.SenderAccount == null || !t.SenderAccount.IsDeleted) &&(t.ReceiverAccount == null || !t.ReceiverAccount.IsDeleted));
			if (filter != null)
			{
			   query=query.Where(filter);
			}

			return await query.ToListAsync();

		}

		public async Task<Transaction?> GetAsync(Expression<Func<Transaction, bool>> filter)
		{
			var query = _context.Transactions.Where(t => (t.SenderAccount == null || !t.SenderAccount.IsDeleted) && (t.ReceiverAccount == null || !t.ReceiverAccount.IsDeleted));
			return await query.Where(filter).FirstOrDefaultAsync();

		}

        public async Task<Transaction?> GetCustomerTransactionByIdAsync(int transactionId, int customerId)
        {
            return await _context.Transactions
                .Include(t => t.SenderAccount)
                .Include(t => t.ReceiverAccount)
                .FirstOrDefaultAsync(t =>
                    t.Id == transactionId &&
                    (
                        (t.SenderAccount != null &&
                         t.SenderAccount.CustomerId == customerId)
                        ||
                        (t.ReceiverAccount != null &&
                         t.ReceiverAccount.CustomerId == customerId)
                    ));
        }

        public async Task<(List<Transaction> Items, int TotalCount)> GetCustomerTransactionsPagedAsync
			(
			int customerId, 
			int pageNumber, 
			int pageSize,
			Expression<Func<Transaction, bool>>? filter
			)
		{
			var query = _context.Transactions.Where(
				t => 
				(t.SenderAccount != null && t.SenderAccount.CustomerId == customerId) 
				|| 
				(t.ReceiverAccount != null && t.ReceiverAccount.CustomerId == customerId));

			if (filter != null)
			{
				query = query.Where(filter);
			}

			var totalCount = await query.CountAsync();

			var items = await query
				.Include(t => t.ReceiverAccount)
				.OrderByDescending(t => t.TransactionDate)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return (items, totalCount);
		}

		public async Task UpdateStatusAsync(int id, TransactionStatus status)
		{
			var transaction = await _context.Transactions.Where(t => (t.SenderAccount == null || !t.SenderAccount.IsDeleted) && (t.ReceiverAccount == null || !t.ReceiverAccount.IsDeleted)).FirstOrDefaultAsync(x => x.Id == id);

			if (transaction != null)
			{
				transaction.UpdateStatus(status);

				await _context.SaveChangesAsync();
			}

		}
	}
}
