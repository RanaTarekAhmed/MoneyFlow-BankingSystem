namespace MoneyFlow.Presentation.ModelVM.Accounts
{
    public class MyAccountsVM
    {
        public List<AccountSummaryVM> Accounts { get; set; } = new();
        public List<TransactionVM> Transactions { get; set; } = new();
    }
}
