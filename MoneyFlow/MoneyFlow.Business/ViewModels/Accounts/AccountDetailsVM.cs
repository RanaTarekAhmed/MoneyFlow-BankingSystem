
namespace MoneyFlow.Business.ViewModels.Accounts
{
    public class AccountDetailsVM
    {
        public AccountSummaryVM Account { get; set; } = new();

        public List<TransactionVM> Transactions { get; set; } = new();
    }
}
