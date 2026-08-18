using MoneyFlow.Data.Enums;


namespace MoneyFlow.Data.Entities
{
	public class Transaction : AuditableEntity
	{
		public int Id { get; private set; }
		public string TransactionNumber { get; private set; } = string.Empty;
		public TransactionType TransactionType { get; private set; }
		public decimal Amount { get; private set; }
		public TransactionStatus Status { get; private set; }
		public DateTime TransactionDate { get; private set; }
		public string? Description { get; private set; }
		public int? EmployeeId { get; private set; }
		public Employee? Employee { get; private set; }
		public int? SenderAccountId { get; private set; }
		public Account? SenderAccount { get; private set; }
		public int? ReceiverAccountId { get; private set; }
		public Account? ReceiverAccount { get; private set; }

		private Transaction()
		{
			// For EF Core
		}

		public Transaction
			(
			string transactionNumber, 
			TransactionType transactionType, 
			decimal amount,
			string? description, 
			int? employeeId, 
			int? senderAccountId, 
			int? receiverAccountId
			)
		{
			var now = DateTime.UtcNow;

			TransactionNumber = transactionNumber;
			TransactionType = transactionType;
			Amount = amount;
            Status = TransactionStatus.Pending;
            TransactionDate = now;
			Description = description;
			EmployeeId = employeeId;
			SenderAccountId = senderAccountId;
			ReceiverAccountId = receiverAccountId;

			CreatedAt = now;
		}

        public void UpdateStatus(TransactionStatus status)
        {
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }
	}
}
