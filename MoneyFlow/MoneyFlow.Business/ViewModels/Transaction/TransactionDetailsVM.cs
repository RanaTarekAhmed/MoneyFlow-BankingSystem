using MoneyFlow.Data.Entities;
using MoneyFlow.Data.Enums;


namespace MoneyFlow.Business.ViewModels.Transaction
{
    public class TransactionDetailsVM
    {
        public string TransactionNumber { get; set; } = string.Empty;
        public TransactionType TransactionType { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Description { get; set; }
        public Account? SenderAccount { get; set; }
        public Account? ReceiverAccount { get; set; }
        public bool IsIncoming { get; set; }
    }
}
