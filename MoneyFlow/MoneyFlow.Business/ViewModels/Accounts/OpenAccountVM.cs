using MoneyFlow.Data.Enums;
using System.ComponentModel.DataAnnotations;


namespace MoneyFlow.Business.ViewModels.Accounts
{
    public class OpenAccountVM
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid customer.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Please select an account type.")]
        public AccountType AccountType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Initial deposit cannot be negative.")]
        public decimal InitialDeposit { get; set; }

        public string? CustomerName { get; set; }
    }
}
