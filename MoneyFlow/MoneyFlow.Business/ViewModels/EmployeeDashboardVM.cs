using MoneyFlow.Data.Enums;

namespace MoneyFlow.Business.ViewModels;

public class EmployeeDashboardVM
{
    public int TotalCustomers { get; set; }
    public int TotalAccounts { get; set; }
    public int TodayDepositsCount { get; set; }
    public int TodayCustomersServed { get; set; }
    public decimal TodayDeposits { get; set; }
    public decimal TodayWithdrawals { get; set; }
    public decimal TodayNetCashMovement => TodayDeposits - TodayWithdrawals;
    public List<EmployeeDashboardTransactionVM> RecentCashOperations { get; set; } = new();
    public List<EmployeeDashboardTransactionVM> RecentActivities { get; set; } = new();
}

public class EmployeeDashboardTransactionVM
{
    public int Id { get; set; }
    public string TransactionNumber { get; set; } = string.Empty;
    public TransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? Description { get; set; }
    public string CustomerName { get; set; } = "Unknown Customer";
    public string AccountNumber { get; set; } = "-";
}