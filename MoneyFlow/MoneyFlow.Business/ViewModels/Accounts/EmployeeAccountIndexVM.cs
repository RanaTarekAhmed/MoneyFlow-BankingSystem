using MoneyFlow.Business.Common;


namespace MoneyFlow.Business.ViewModels.Accounts
{
    public class EmployeeAccountIndexVM
    {
        public EmployeeAccountSummaryVM Summary { get; set; } = new();
        public PagedResult<EmployeeAccountVM> Accounts { get; set; } = null!;
        public AccountQueryVM? Query { get; set; } = new();
    }
}
