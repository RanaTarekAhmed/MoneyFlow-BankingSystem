using MoneyFlow.Business.Common;


namespace MoneyFlow.Business.ViewModels.Transaction
{
    public class EmployeeTransactionIndexVM
    {
        public PagedResult<EmployeeTransactionVM> Transactions { get; set; } = null!;
        public TransactionQueryVM? Query { get; set; } = new();
    }
}
