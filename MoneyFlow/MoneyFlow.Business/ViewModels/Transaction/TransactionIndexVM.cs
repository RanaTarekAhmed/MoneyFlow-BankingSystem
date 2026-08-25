using MoneyFlow.Business.Common;


namespace MoneyFlow.Business.ViewModels.Transaction
{
    public class TransactionIndexVM
    {
        public PagedResult<TransactionVM> Transactions { get; set; } = null!;
        public TransactionQueryVM? Query { get; set; } = new();
    }
}
