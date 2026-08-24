using MoneyFlow.Data.Enums;

namespace MoneyFlow.Presentation.ModelVM.Accounts
{
    public class AccountSummaryVM
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public AccountType AccountType { get; set; }
        public AccountStatus Status { get; set; }
        public decimal Balance { get; set; }
        public DateTime OpenDate { get; set; }
    }
}
