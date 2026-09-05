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
		private readonly ITransactionService _transactionService;

		public AccountService(
			IAccountRepository accountRepository,
			ITransactionRepository transactionRepository,
			ICustomerRepository customerRepository,
			ITransactionService transactionService)
		{
			_accountRepository = accountRepository;
			_transactionRepository = transactionRepository;
			_customerRepository = customerRepository;
			_transactionService = transactionService;
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

	
			return new MyAccountsVM
			{
				Accounts = accountVMs,
				
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

            var senderAccount = await _accountRepository.GetAsync(a =>a.Id == model.SenderAccountId &&a.CustomerId == customer.Id);

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

            // Check sender account status
            if (senderAccount.Status != AccountStatus.Active)
            {
                return (false, "The sender account is inactive.", null);
            }

            // Check receiver account status
            if (receiverAccount.Status != AccountStatus.Active)
            {
                return (false, "The receiver account is inactive.", null);
            }

            try
            {
                senderAccount.Transfer(receiverAccount, model.Amount);
            }
            catch (ArgumentException ex)
            {
                return (false, ex.Message, null);
            }
            catch (InvalidOperationException ex)
            {
                return (false, ex.Message, null);
            }

            var transactionNumber = await _transactionService.GenerateTransactionNumberAsync();

            var transaction = new Transaction(
                transactionNumber,
                TransactionType.Transfer,
                model.Amount,
                model.Description,
                null,
                senderAccount.Id,
                receiverAccount.Id);

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
				else if (account.Status == AccountStatus.Suspended)
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
				return (false,"Please enter the account number.",null);
			}

			if (model.Amount <= 0)
			{
				return (false,"Deposit amount must be greater than zero.",null);
			}

			var account = await _accountRepository.GetAsync(a => a.AccountNumber == model.AccountNumber.Trim());

			if (account == null)
			{
				return (false,"The account was not found.",null);
			}

			if (account.Status != AccountStatus.Active)
			{
				return (false,"The account is not active.",null);
			}

			try
			{
				account.Deposit(model.Amount);
			}
			catch (ArgumentException ex)
			{
				return (false, ex.Message, null);
			}

			var transactionNumber = await _transactionService.GenerateTransactionNumberAsync();

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
				return (false,"Please enter the account number.",null);
			}

			if (model.Amount <= 0)
			{
				return (false,"Withdrawal amount must be greater than zero.",null);
			}

			var account = await _accountRepository.GetAsync(a => a.AccountNumber == model.AccountNumber.Trim());

			if (account == null)
			{
				return (false,"The account was not found.",null);
			}

			if (account.Status != AccountStatus.Active)
			{
				return (false,"The account is not active.",null);
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

			var transactionNumber = await _transactionService.GenerateTransactionNumberAsync();

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

        public async Task<bool> OpenAccountAsync(OpenAccountVM model)
        {
            var customer = await _customerRepository.GetAsync(c => c.Id == model.CustomerId);

            if (customer == null)
            {
                return false;
            }

            if (model.AccountType != AccountType.Current &&
                model.AccountType != AccountType.Savings)
            {
                return false;
            }

            if (model.InitialDeposit < 0)
            {
                return false;
            }

            var accountNumber = await GenerateAccountNumberAsync();

            var account = new Account(
                accountNumber,
                model.AccountType,
                model.CustomerId
            );

            await _accountRepository.AddAsync(account);

            Transaction? transaction = null;

            if (model.InitialDeposit > 0)
            {
                account.Deposit(model.InitialDeposit);

                var transactionNumber =
                    await _transactionService.GenerateTransactionNumberAsync();

                transaction = new Transaction(
                    transactionNumber,
                    TransactionType.Deposit,
                    model.InitialDeposit,
                    "Initial deposit",
                    null,
                    null,
                    account.Id
                );

                transaction.UpdateStatus(TransactionStatus.Completed);

                await _accountRepository.UpdateAsync(account);
                await _transactionRepository.AddAsync(transaction);
            }

            return true;
        }

        private async Task<string> GenerateAccountNumberAsync()
		{
			string accountNumber;

			do
			{
				accountNumber = $"MF-{Random.Shared.NextInt64(1000000000, 10000000000)}";
			}
			while (await _accountRepository.AnyAsync(a => a.AccountNumber == accountNumber));

			return accountNumber;
		}

		public async Task<EmployeeAccountDetailsVM?> GetAccountDetailsAsync(int accountId)
		{
			var account = await _accountRepository.GetAccountDetailsAsync(accountId);
			if (account == null)
			{
				return null;
			}

			var transactions = await _transactionRepository.GetAllAsync(t => t.ReceiverAccountId == accountId || t.SenderAccountId == accountId);

			var transactionVMs = transactions
				.OrderByDescending(t => t.TransactionDate)
				.Select(t => new ViewModels.Transaction.TransactionVM
				{
					Id = t.Id,
					TransactionNumber = t.TransactionNumber,
					TransactionType = t.TransactionType,
					Amount = t.Amount,
					Status = t.Status,
					TransactionDate = t.TransactionDate,
					Description = t.Description,
					IsIncoming = t.ReceiverAccountId == accountId
				})
				.ToList();

			return new EmployeeAccountDetailsVM
			{
				AccountNumber = account.AccountNumber,
				AccountType = account.AccountType,
				Status = account.Status,
				Balance = account.Balance,
				OpenDate = account.OpenDate,
				CustomerId = account.CustomerId,
				CustomerName = account.Customer.User.FirstName + " " + account.Customer.User.LastName,
				CustomerEmail = account.Customer.User.Email ?? "",
				RecentTransactions = transactionVMs
			};
		}

		public async Task<bool> UpdateStatusAsync(UpdateStatusVM model)
		{
			var account = await _accountRepository.GetAsync(a => a.Id == model.AccountId);

			if (account == null)
			{
				return false;
			}

			account.Update(account.AccountType, model.Status);

			await _accountRepository.UpdateAsync(account);

			return true;
		}
	}
}