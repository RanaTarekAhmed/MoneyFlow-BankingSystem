

namespace MoneyFlow.Business.ViewModels.Employee
{
    public class EmployeeProfileVM
    {
        public ProfileInformationVM ProfileInformation { get; set; } = new();
        public ChangePasswordVM ChangePassword { get; set; } = new();
    }
}
