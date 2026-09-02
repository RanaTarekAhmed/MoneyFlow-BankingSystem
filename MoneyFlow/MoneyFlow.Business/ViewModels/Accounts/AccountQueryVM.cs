using MoneyFlow.Data.Enums;


namespace MoneyFlow.Business.ViewModels.Accounts
{
    public class AccountQueryVM
    {
        public string? Search { get; set; }
        public AccountType? AccountType { get; set; }
        public AccountStatus? Status { get; set; }
    }
}
