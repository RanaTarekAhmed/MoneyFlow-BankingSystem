using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MoneyFlow.Business.ViewModels.Accounts
{
    public class TransferVM
    {
        [Required(ErrorMessage = "Please select the account to transfer from.")]
        public int SenderAccountId { get; set; }

        [Required(ErrorMessage = "Please enter the receiver account number.")]
        public string ReceiverAccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the transfer amount.")]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335",
            ErrorMessage = "Transfer amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [StringLength(250, ErrorMessage = "Description cannot exceed 250 characters.")]
        public string? Description { get; set; }
        public List<AccountTransferOptionVM> Accounts { get; set; } = new();
    }

    public class AccountTransferOptionVM
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}
