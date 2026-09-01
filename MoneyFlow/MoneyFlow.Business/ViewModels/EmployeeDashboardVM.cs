using MoneyFlow.Business.Common;
using MoneyFlow.Data.Enums;

namespace MoneyFlow.Business.ViewModels;

public class EmployeeDashboardVM
{
    public int TotalCustomers { get; set; }
    public int TotalAccounts { get; set; }
    public int TodayTransactionsCount { get; set; }
    public decimal TodayDeposits { get; set; }
    public decimal TodayWithdrawals { get; set; }
    public PagedResult<EmployeeDashboardTransactionVM> Transactions { get; set; } = new();
}

public class EmployeeDashboardTransactionVM
{
    public string TransactionNumber { get; set; } = string.Empty;
    public TransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTime TransactionDate { get; set; }
    public string CustomerName { get; set; } = "Unknown Customer";
    public string AccountNumber { get; set; } = "-";
}