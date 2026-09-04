using MoneyFlow.Data.Enums;


namespace MoneyFlow.Business.ViewModels.Accounts
{
    public class EmployeeAccountDetailsVM
    {
        public string AccountNumber { get; set; } = string.Empty;
        public AccountType AccountType { get; set; }
        public AccountStatus Status { get; set; }
        public decimal Balance { get; set; }
        public DateTime OpenDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public List<Transaction.TransactionVM> RecentTransactions { get; set; } = [];
    }
}
