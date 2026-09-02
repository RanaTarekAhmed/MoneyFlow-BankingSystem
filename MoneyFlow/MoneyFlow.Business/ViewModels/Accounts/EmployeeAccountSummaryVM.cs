

namespace MoneyFlow.Business.ViewModels.Accounts
{
    public class EmployeeAccountSummaryVM
    {
        public decimal TotalDepositsHeld { get; set; }
        public int ActiveCurrentAccounts { get; set; }
        public int ActiveSavingsAccounts { get; set; }
        public int SuspendedAccounts { get; set; }
    }
}
