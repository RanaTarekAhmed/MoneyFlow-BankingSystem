using Microsoft.AspNetCore.Identity;
using MoneyFlow.Business.Services.Interfaces;
using MoneyFlow.Business.ViewModels.Customer;
using MoneyFlow.Business.ViewModels;
using MoneyFlow.Business.Common;
using MoneyFlow.Data.Entities;
using MoneyFlow.Data.Enums;
using MoneyFlow.Data.Repositories.Interfaces;

namespace MoneyFlow.Business.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;

        public DashboardService(
            UserManager<ApplicationUser> userManager,
            ICustomerRepository customerRepository,
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository)
        {
            _userManager = userManager;
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<DashboardVM?> GetDashboardAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            var customer = await _customerRepository.GetAsync(c => c.UserId == userId);

            if (customer == null)
            {
                return null;
            }

            var accounts = await _accountRepository.GetAllAsync(a => a.CustomerId == customer.Id && a.Status != AccountStatus.Closed);
            var accountIds = accounts.Select(a => a.Id).ToList();

            // Only show the two most recently opened active accounts in the dashboard
            // view All Accounts -> to view full account list 
            var displayedAccounts = accounts.Where(a => a.Status == AccountStatus.Active).OrderByDescending(a => a.OpenDate).Take(2).ToList();
            var transactions = accountIds.Count == 0 ? new List<Transaction>() : await _transactionRepository.GetAllAsync(t => (t.SenderAccountId.HasValue && accountIds.Contains(t.SenderAccountId.Value)) || (t.ReceiverAccountId.HasValue && accountIds.Contains(t.ReceiverAccountId.Value)));

            var recentTransactions = transactions
                .OrderByDescending(t => t.TransactionDate)
                .Take(4)
                .Select(t =>
                {
                    var isIncoming = t.ReceiverAccountId.HasValue &&
                                     accountIds.Contains(t.ReceiverAccountId.Value);

                    return new DashboardTransactionVM
                    {
                        Id = t.Id,
                        TransactionNumber = t.TransactionNumber,
                        TransactionType = t.TransactionType,
                        Amount = t.Amount,
                        Status = t.Status,
                        TransactionDate = t.TransactionDate,
                        IsIncoming = isIncoming,
                        Title = t.TransactionType switch
                        {
                            TransactionType.Deposit => "Cash Deposit",
                            TransactionType.Withdrawal => "Cash Withdrawal",
                            _ => isIncoming ? "Transfer from Account" : "Transfer to Account"
                        }
                    };
                })
                .ToList();

            var weekAgo = DateTime.UtcNow.AddDays(-7);

            return new DashboardVM
            {
                CustomerName = string.IsNullOrWhiteSpace(user.FirstName) ? user.UserName ?? "Customer" : user.FirstName,

                TotalBalance = accounts.Where(a => a.Status == AccountStatus.Active).Sum(a => a.Balance),

                ActiveAccounts = accounts.Count(a => a.Status == AccountStatus.Active),

                RecentTransactionsCount = transactions.Count(t => t.TransactionDate >= weekAgo),

                Accounts = displayedAccounts.Select(a => new DashboardAccountVM
                {
                    Id = a.Id,
                    AccountNumber = a.AccountNumber,
                    AccountType = a.AccountType,
                    Status = a.Status,
                    Balance = a.Balance,
                    OpenDate = a.OpenDate,
                    AccountTypeName = a.AccountType == AccountType.Current ? "Checking" : "Savings",
                    AccountTypeClass = a.AccountType == AccountType.Current ? "checking" : "savings",
                    MaskedAccountNumber = a.AccountNumber.Length > 4 ? "•••• " + a.AccountNumber[^4..] : a.AccountNumber,
                    StatusName = a.Status.ToString()
                }).ToList(),

                RecentTransactions = recentTransactions
            };
        }
        public async Task<EmployeeDashboardVM> GetEmployeeDashboardAsync(int pageNumber, int pageSize)
        {
            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 50);

            var today = DateTime.UtcNow.Date;
            var customers = await _customerRepository.GetAllAsync(null);
            var accounts = await _accountRepository.GetAllAsync(null);

            // Only completed transactions count as processed cash operations
            var todayTransactions = await _transactionRepository.GetAllAsync(t =>
                t.TransactionDate >= today &&
                t.Status == TransactionStatus.Completed);
            
            var todayCashOperations = todayTransactions.Where(t => t.TransactionType == TransactionType.Deposit || t.TransactionType == TransactionType.Withdrawal).ToList();
            var recentCashOperations = await _transactionRepository.GetAllTransactionsPagedAsync(
                1,
                5,
                t => t.TransactionType == TransactionType.Deposit || t.TransactionType == TransactionType.Withdrawal);
            
            var recentActivities = await _transactionRepository.GetAllTransactionsPagedAsync(
                1,
                5,
                null);

            var todayCustomersServed = todayCashOperations
                .SelectMany(t => new[]
                {
                    t.SenderAccount?.CustomerId,
                    t.ReceiverAccount?.CustomerId
                })
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .Count();

            return new EmployeeDashboardVM
            {
                TotalCustomers = customers.Count,
                TotalAccounts = accounts.Count,
                TodayDepositsCount = todayCashOperations.Count(t => t.TransactionType == TransactionType.Deposit),
                TodayCustomersServed = todayCustomersServed,
                TodayDeposits = todayCashOperations
                    .Where(t => t.TransactionType == TransactionType.Deposit)
                    .Sum(t => t.Amount),
                TodayWithdrawals = todayCashOperations
                    .Where(t => t.TransactionType == TransactionType.Withdrawal)
                    .Sum(t => t.Amount),
                RecentCashOperations = recentCashOperations.Items.Select(MapEmployeeDashboardTransaction).ToList(),
                RecentActivities = recentActivities.Items.Select(MapEmployeeDashboardTransaction).ToList()
            };
        }

        private static EmployeeDashboardTransactionVM MapEmployeeDashboardTransaction(Transaction t)
        {
            var account = t.TransactionType == TransactionType.Deposit ? t.ReceiverAccount : t.SenderAccount ?? t.ReceiverAccount;
            var customer = account?.Customer?.User;
            var customerName = customer == null ? "Unknown Customer" : $"{customer.FirstName} {customer.LastName}".Trim();
            return new EmployeeDashboardTransactionVM
            {
                Id = t.Id,
                TransactionNumber = t.TransactionNumber,
                TransactionType = t.TransactionType,
                Amount = t.Amount,
                Status = t.Status,
                TransactionDate = t.TransactionDate,
                Description = t.Description,
                AccountNumber = account?.AccountNumber ?? "-",
                CustomerName = string.IsNullOrWhiteSpace(customerName) ? "Unknown Customer" : customerName
            };
        }

    }
}