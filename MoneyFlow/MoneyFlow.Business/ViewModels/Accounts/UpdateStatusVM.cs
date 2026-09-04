using MoneyFlow.Data.Enums;


namespace MoneyFlow.Business.ViewModels.Accounts
{
    public class UpdateStatusVM
    {
        public int AccountId { get; set; }
        public string? AccountNumber { get; set; }
        public AccountStatus Status { get; set; }
    }
}
