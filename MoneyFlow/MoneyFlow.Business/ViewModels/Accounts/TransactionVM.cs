using MoneyFlow.Data.Enums;

namespace MoneyFlow.Business.ViewModels.Accounts
{
    public class TransactionVM
    {
        public int Id { get; set; }
        public string TransactionNumber { get; set; } = string.Empty;
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Description { get; set; }
        public int? SenderAccountId { get; set; }
        public int? ReceiverAccountId { get; set; }
    }
}
