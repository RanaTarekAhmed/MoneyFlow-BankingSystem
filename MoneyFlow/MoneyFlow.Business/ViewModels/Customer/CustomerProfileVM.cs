

namespace MoneyFlow.Business.ViewModels.Customer
{
    public class CustomerProfileVM
    {
        public ProfileInformationVM ProfileInformation { get; set; } = new();
        public ChangePasswordVM ChangePassword { get; set; } = new();
    }
}
