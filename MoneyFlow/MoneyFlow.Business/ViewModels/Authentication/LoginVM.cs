

using System.ComponentModel.DataAnnotations;

namespace MoneyFlow.Business.ViewModels.Authentication
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Please enter your email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter your password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
