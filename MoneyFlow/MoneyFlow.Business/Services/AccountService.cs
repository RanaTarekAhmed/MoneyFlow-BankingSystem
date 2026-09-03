using MoneyFlow.Business.Services.Interfaces;
using MoneyFlow.Business.ViewModels.Accounts;
using MoneyFlow.Data.Enums;
using MoneyFlow.Data.Repositories.Interfaces;
using MoneyFlow.Data.Entities;
using MoneyFlow.Business.Common;
using System.Linq.Expressions;
namespace MoneyFlow.Business.Services
{
	public class AccountService : IAccountService
	{
		private readonly IAccountRepository _accountRepository;
		private readonly ITransactionRepository _transactionRepository;
		private readonly ICustomerRepository _customerRepository;

		public AccountService(
			IAccountRepository accountRepository,
			ITransactionRepository transactionRepository,
			ICustomerRepository customerRepository)
		{
			_accountRepository = accountRepository;
			_transactionRepository = transactionRepository;
			_customerRepository = customerRepository;
		}

		public async Task<MyAccountsVM?> GetMyAccountsAsync(string userId)
		{
			var customer = await _customerRepository.GetAsync(
				c => c.UserId == userId
			);

			if (customer == null)
			{
				return null;
			}

			var accounts = await _accountRepository.GetAllAsync(
				a => a.CustomerId == customer.Id
			);

			var accountVMs = accounts.Select(a => new AccountSummaryVM
			{
				Id = a.Id,
				AccountNumber = a.AccountNumber,
				AccountType = a.AccountType,
				Status = a.Status,
				Balance = a.Balance,
				OpenDate = a.OpenDate
			}).ToList();

			var accountIds = accounts
				.Select(a => a.Id)
				.ToList();

			var transactions = await _transactionRepository.GetAllAsync(t => (t.SenderAccountId.HasValue && accountIds.Contains(t.SenderAccountId.Value)) || (t.ReceiverAccountId.HasValue && accountIds.Contains(t.ReceiverAccountId.Value)) );

			var transactionVMs = transactions.Select(t => new TransactionVM
			{
				Id = t.Id,
				TransactionNumber = t.TransactionNumber,
				TransactionType = t.TransactionType,
				Amount = t.Amount,
				Status = t.Status,
				TransactionDate = t.TransactionDate,
				Description = t.Description,
				SenderAccountId = t.SenderAccountId,
				ReceiverAccountId = t.ReceiverAccountId
			}).ToList();

			return new MyAccountsVM
			{
				Accounts = accountVMs,
				Transactions = transactionVMs
			};
		}

		public async Task<AccountDetailsVM?> GetAccountDetailsAsync( string userId, int accountId)
		{
			var customer = await _customerRepository.GetAsync(c => c.UserId == userId );

			if (customer == null)
			{
				return null;
			}

			var account = await _accountRepository.GetAsync( a => a.Id == accountId &&a.CustomerId == customer.Id );

			if (account == null)
			{
				return null;
			}

			var transactions = await _transactionRepository.GetAllAsync( t => t.SenderAccountId == accountId ||  t.ReceiverAccountId == accountId);

			transactions = transactions.OrderByDescending(t => t.TransactionDate).ToList();

			return new AccountDetailsVM
			{
				Account = new AccountSummaryVM
				{
					Id = account.Id,
					AccountNumber = account.AccountNumber,
					AccountType = account.AccountType,
					Status = account.Status,
					Balance = account.Balance,
					OpenDate = account.OpenDate
				},

				Transactions = transactions.Select(t => new TransactionVM
				{
					Id = t.Id,
					TransactionNumber = t.TransactionNumber,
					TransactionType = t.TransactionType,
					Amount = t.Amount,
					Status = t.Status,
					TransactionDate = t.TransactionDate,
					Description = t.Description,
					SenderAccountId = t.SenderAccountId,
					ReceiverAccountId = t.ReceiverAccountId
				}).ToList()
			};
		}

		public async Task<AccountDetailsVM?> GetAccountByNumberAsync(string accountNumber)
		{
			if (string.IsNullOrWhiteSpace(accountNumber))
			{
				return null;
			}

			var account = await _accountRepository.GetAsync(a => 
				a.AccountNumber == accountNumber.Trim() && 
				a.Status == AccountStatus.Active);

			if (account == null)
			{
				return null;
			}

			var transactions = await _transactionRepository.GetAllAsync(t => 
				t.SenderAccountId == account.Id || t.ReceiverAccountId == account.Id);

			transactions = transactions.OrderByDescending(t => t.TransactionDate).ToList();

			return new AccountDetailsVM
			{
				Account = new AccountSummaryVM
				{
					Id = account.Id,
					AccountNumber = account.AccountNumber,
					AccountType = account.AccountType,
					Status = account.Status,
					Balance = account.Balance,
					OpenDate = account.OpenDate
				},

				Transactions = transactions.Select(t => new TransactionVM
				{
					Id = t.Id,
					TransactionNumber = t.TransactionNumber,
					TransactionType = t.TransactionType,
					Amount = t.Amount,
					Status = t.Status,
					TransactionDate = t.TransactionDate,
					Description = t.Description,
					SenderAccountId = t.SenderAccountId,
					ReceiverAccountId = t.ReceiverAccountId
				}).ToList()
			};
		}

		public async Task<TransferVM?> GetTransferModelAsync(string userId)
		{
			var customer = await _customerRepository.GetAsync(c => c.UserId == userId);

			if (customer == null)
			{
				return null;
			}

			var accounts = await _accountRepository.GetAllAsync(a => a.CustomerId == customer.Id);

			var model = new TransferVM
			{
				Accounts = accounts.Select(a => new AccountTransferOptionVM
				{ Id = a.Id, AccountNumber = a.AccountNumber, Balance = a.Balance }).ToList()
			};
			return model;
		}


		public async Task<(bool Success, string Message, Transaction? Transaction)> TransferAsync(string userId,TransferVM model)
		{
			var customer = await _customerRepository.GetAsync(c => c.UserId == userId);

			if (customer == null)
			{
				return (false, "Customer was not found.", null);
			}

			var senderAccount = await _accountRepository.GetAsync(a => a.Id == model.SenderAccountId && a.CustomerId == customer.Id);

			if (senderAccount == null)
			{
				return (false,"The selected sender account does not belong to you.",null);
			}

			if (string.IsNullOrWhiteSpace(model.ReceiverAccountNumber))
			{
				return (false,"Please enter the receiver account number.",null);
			}

			var receiverAccount = await _accountRepository.GetAsync(a => a.AccountNumber == model.ReceiverAccountNumber.Trim());

			if (receiverAccount == null)
			{
				return (false,"The receiver account was not found.",null);
			}

			try
			{
				senderAccount.Transfer(receiverAccount,model.Amount);
			}
			catch (ArgumentException ex)
			{
				return (false, ex.Message, null);
			}
			catch (InvalidOperationException ex)
			{
				return (false, ex.Message, null);
			}

			var transactionNumber = $"TRX-{Guid.NewGuid():N}".ToUpper();

			var transaction = new Transaction(transactionNumber,TransactionType.Transfer,model.Amount, model.Description,null,senderAccount.Id,receiverAccount.Id);

			transaction.UpdateStatus(TransactionStatus.Completed);

			await _accountRepository.UpdateAsync(senderAccount);

			await _accountRepository.UpdateAsync(receiverAccount);

			await _transactionRepository.AddAsync(transaction);

			return (true,"Transfer completed successfully.",transaction);
		}

		public async Task<PagedResult<EmployeeAccountVM>> GetAllAccountsPagedAsync(int pageNumber, int pageSize, AccountQueryVM? query)
		{
			Expression<Func<Account, bool>>? filter = null;

			if (query != null)
			{
				var search = query.Search?.Trim();

				if (!string.IsNullOrEmpty(search) 
					|| query.AccountType.HasValue 
					|| query.Status.HasValue
					)
				{
					filter = a =>
					(string.IsNullOrEmpty(search) || a.AccountNumber.Contains(search)
					|| a.Customer.User.FirstName.Contains(search)
					|| a.Customer.User.LastName.Contains(search)
					|| (a.Customer.User.FirstName + " " + a.Customer.User.LastName).Contains(search))
					&&
					(!query.AccountType.HasValue || a.AccountType == query.AccountType)
					&&
					(!query.Status.HasValue || a.Status == query.Status);
				}
			}

			var (accounts, totalCount) = await _accountRepository.GetAllAccountsPagedAsync(pageNumber, pageSize, filter);

			var accountVMs = accounts
				.Select(a => new EmployeeAccountVM
				{
					Id = a.Id,
					AccountNumber = a.AccountNumber,
					AccountType = a.AccountType,
					Status = a.Status,
					Balance = a.Balance,
					OpenDate = a.OpenDate,
					CustomerName = a.Customer.User.FirstName + " " + a.Customer.User.LastName,
					CustomerEmail = a.Customer.User.Email ?? ""
				})
				.ToList();

			return new PagedResult<EmployeeAccountVM>
			{
				Items = accountVMs,
				PageNumber = pageNumber,
				PageSize = pageSize,
				TotalCount = totalCount
			};
		}

        public async Task<EmployeeAccountSummaryVM> GetAllAccountsSummaryAsync()
        {
			var accounts = await _accountRepository.GetAllAsync();

			decimal totalDepositsHeld = 0;
			int activeCurrentAccounts = 0;
			int activeSavingsAccounts = 0;
			int suspendedAccounts = 0;

			foreach (var account in accounts)
			{
				totalDepositsHeld += account.Balance;
				if (account.Status == AccountStatus.Active)
				{
					if (account.AccountType == AccountType.Current)
					{
                        activeCurrentAccounts++;
                    }
					else
					{
						activeSavingsAccounts++;
					}
				}
				else if(account.Status == AccountStatus.Suspended)
				{
					suspendedAccounts++;
				}
			}

			return new EmployeeAccountSummaryVM
			{
				TotalDepositsHeld = totalDepositsHeld,
				ActiveCurrentAccounts = activeCurrentAccounts,
				ActiveSavingsAccounts = activeSavingsAccounts,
				SuspendedAccounts = suspendedAccounts
			};
        }


        public async Task<(bool Success, string Message, Transaction? Transaction)> DepositAsync(CashOperationVM model)
        {
            if (string.IsNullOrWhiteSpace(model.AccountNumber))
            {
                return (false, "Please enter the account number.", null);
            }

            if (model.Amount <= 0)
            {
                return (false, "Deposit amount must be greater than zero.", null);
            }

            var account = await _accountRepository.GetAsync(a => a.AccountNumber == model.AccountNumber.Trim());

            if (account == null)
            {
                return (false, "The account was not found.", null);
            }

            if (account.Status != AccountStatus.Active)
            {
                return (false, "The account is not active.", null);
            }

            try
            {
                account.Deposit(model.Amount);
            }
            catch (ArgumentException ex)
            {
                return (false, ex.Message, null);
            }

            var transactionNumber = $"TRX-{Guid.NewGuid():N}".ToUpper();

            var transaction = new Transaction(
                transactionNumber,
                TransactionType.Deposit,
                model.Amount,
                model.Description,
                null,
                null,
                account.Id
            );

            transaction.UpdateStatus(TransactionStatus.Completed);

            await _accountRepository.UpdateAsync(account);

            await _transactionRepository.AddAsync(transaction);

            return (true,"Deposit completed successfully.",transaction);
        }


        public async Task<(bool Success, string Message, Transaction? Transaction)> WithdrawAsync(CashOperationVM model)
        {
            if (string.IsNullOrWhiteSpace(model.AccountNumber))
            {
                return (false, "Please enter the account number.", null);
            }

            if (model.Amount <= 0)
            {
                return (false, "Withdrawal amount must be greater than zero.", null);
            }

            var account = await _accountRepository.GetAsync(a => a.AccountNumber == model.AccountNumber.Trim());

            if (account == null)
            {
                return (false, "The account was not found.", null);
            }

            if (account.Status != AccountStatus.Active)
            {
                return (false, "The account is not active.", null);
            }

            try
            {
                account.Withdraw(model.Amount);
            }
            catch (ArgumentException ex)
            {
                return (false, ex.Message, null);
            }
            catch (InvalidOperationException ex)
            {
                return (false, ex.Message, null);
            }

            var transactionNumber = $"TRX-{Guid.NewGuid():N}".ToUpper();

            var transaction = new Transaction(
                transactionNumber,
                TransactionType.Withdrawal,
                model.Amount,
                model.Description,
                null,
                account.Id,
                null
            );

            transaction.UpdateStatus(TransactionStatus.Completed);

            await _accountRepository.UpdateAsync(account);

            await _transactionRepository.AddAsync(transaction);

            return (true,"Withdrawal completed successfully.",transaction);
        }
    }
}