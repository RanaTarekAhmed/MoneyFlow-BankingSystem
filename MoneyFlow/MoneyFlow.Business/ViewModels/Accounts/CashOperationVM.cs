using System.ComponentModel.DataAnnotations;


namespace MoneyFlow.Business.ViewModels.Accounts
{
    public class CashOperationVM
    {
        [Required]
        public string OperationType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the account number.")]
        public string AccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the amount.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [StringLength(250, ErrorMessage = "Transaction memo cannot exceed 250 characters.")]
        public string? Description { get; set; }
    }
}
