

using System.ComponentModel.DataAnnotations;

namespace MoneyFlow.Business.ViewModels.Customer
{
    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "Please enter your current password.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your new password.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your new password.")]
        [Compare("NewPassword", ErrorMessage = "The new passwords do not match.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
