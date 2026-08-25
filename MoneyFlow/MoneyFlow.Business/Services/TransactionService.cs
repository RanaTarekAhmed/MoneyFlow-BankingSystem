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

        public async Task<TransactionDetailsVM?> GetCustomerTransactionByIdAsync(int transactionId, string? userId)
        {
            var customer = await _customerRepository.GetAsync(c => c.UserId == userId);

            if (customer == null)
            {
                return null;
            }

            var customerId = customer.Id;

			var transaction = await _transactionRepository.GetCustomerTransactionByIdAsync(transactionId, customerId);

            if (transaction == null)
            {
                return null;
            }

			var transactionDetailsVM = new TransactionDetailsVM
			{
				TransactionNumber = transaction.TransactionNumber,
				TransactionType = transaction.TransactionType,
				Amount = transaction.Amount,
				Status = transaction.Status,
				TransactionDate = transaction.TransactionDate,
				Description = transaction.Description,
				SenderAccount = transaction.SenderAccount,
				ReceiverAccount = transaction.ReceiverAccount,
				IsIncoming = transaction.ReceiverAccount?.CustomerId == customerId
			};

			return transactionDetailsVM;
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
                throw new InvalidOperationException("Customer not found.");
            }

            var customerId = customer.Id;

			var (transactions, totalCount) = await _transactionRepository.GetCustomerTransactionsPagedAsync(customerId, pageNumber, pageSize, filter);

			var transactionVMs = transactions
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
				Items = transactionVMs,
				PageNumber = pageNumber,
				PageSize = pageSize,
				TotalCount = totalCount
			};
		}
	}
}
