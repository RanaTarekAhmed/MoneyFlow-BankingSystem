namespace MoneyFlow.Presentation.ModelVM.Accounts
{
    public class AccountDetailsVM
    {
        public AccountSummaryVM Account { get; set; } = new();

        public List<TransactionVM> Transactions { get; set; } = new();
    }
}
