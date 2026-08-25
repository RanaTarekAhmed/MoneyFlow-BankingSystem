namespace MoneyFlow.Business.ViewModels.Customer
{
    public class DashboardVM
    {
        public string CustomerName { get; set; } = "Customer";
        public decimal TotalBalance { get; set; }
        public int ActiveAccounts { get; set; }
        public int RecentTransactionsCount { get; set; }
        public List<DashboardAccountVM> Accounts { get; set; } = new();
        public List<DashboardTransactionVM> RecentTransactions { get; set; } = new();
    }

    public class DashboardAccountVM
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public MoneyFlow.Data.Enums.AccountType AccountType { get; set; }
        public MoneyFlow.Data.Enums.AccountStatus Status { get; set; }
        public decimal Balance { get; set; }
        public DateTime OpenDate { get; set; }
        public string AccountTypeName { get; set; } = string.Empty;
        public string AccountTypeClass { get; set; } = string.Empty;
        public string MaskedAccountNumber { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
    }

    public class DashboardTransactionVM
    {
        public int Id { get; set; }
        public string TransactionNumber { get; set; } = string.Empty;
        public MoneyFlow.Data.Enums.TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public MoneyFlow.Data.Enums.TransactionStatus Status { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool IsIncoming { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}