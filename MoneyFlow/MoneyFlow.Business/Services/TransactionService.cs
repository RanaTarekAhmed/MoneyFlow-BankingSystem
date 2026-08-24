using MoneyFlow.Business.Common;
using MoneyFlow.Business.Services.Interfaces;
using MoneyFlow.Business.ViewModels.Transaction;
using MoneyFlow.Data.Repositories;
using System.Linq.Expressions;
using MoneyFlow.Data.Entities;
using MoneyFlow.Data.Repositories.Interfaces;


namespace MoneyFlow.Business.Services
{
	public class TransactionService : ITransactionService
	{
		private readonly ITransactionRepository _transactionRepository;
		private readonly ICustomerRepository _customerRepository;

        public TransactionService(ITransactionRepository transactionRepository, ICustomerRepository customerRepository)
        {
            _transactionRepository = transactionRepository;
            _customerRepository = customerRepository;
        }

        public async Task<PagedResult<TransactionVM>> GetCustomerTransactionsPagedAsync
			(
			string? userId, 
			int pageNumber, 
			int pageSize, 
			Expression<Func<Transaction, bool>>? filter)
		{
			var customer = await _customerRepository.GetAsync(c => c.UserId == userId);

			if (customer == null)
			{
				throw new Exception("Customer not found.");
			}

			var customerId = customer.Id;

			var (transactions, totalCount) = await _transactionRepository.GetCustomerTransactionsPagedAsync(customerId, pageNumber, pageSize, filter);

			var items = transactions
				.Select(t => new TransactionVM
				{
					Id = t.Id,
					TransactionNumber = t.TransactionNumber,
					TransactionType = t.TransactionType,
					Amount = t.Amount,
					Status = t.Status,
					TransactionDate = t.TransactionDate,
					Description = t.Description,
					IsIncoming = t.ReceiverAccount?.CustomerId == customerId
				})
				.ToList();

			return new PagedResult<TransactionVM>
			{
				Items = items,
				PageNumber = pageNumber,
				PageSize = pageSize,
				TotalCount = totalCount
			};
		}
	}
}
