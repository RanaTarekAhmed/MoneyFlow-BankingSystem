using MoneyFlow.Business.Services.Interfaces;
using MoneyFlow.Business.ViewModels.Accounts;
using MoneyFlow.Data.Enums;
using MoneyFlow.Data.Repositories.Interfaces;
using MoneyFlow.Data.Entities;
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
    }
}