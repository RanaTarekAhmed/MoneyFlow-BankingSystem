using MoneyFlow.Data.Enums;


namespace MoneyFlow.Data.Entities
{
	public class Account : AuditableEntity
	{
		public int Id { get; private set; }
		public string AccountNumber { get; private set; } = string.Empty;
		public AccountType AccountType { get; private set; }
		public AccountStatus Status { get; private set; }
		public decimal Balance { get; private set; }
		public DateTime OpenDate { get; private set; }
		public int CustomerId { get; private set; }
		public Customer Customer { get; private set; } = null!;
		public List<Transaction> SentTransactions { get; private set; } = new List<Transaction>();
		public List<Transaction> ReceivedTransactions { get; private set; } = new List<Transaction>();

		private Account()
		{
			// For EF Core
		}

		public Account(string accountNumber, AccountType accountType, int customerId)
		{
			var now = DateTime.UtcNow;

			AccountNumber = accountNumber;
			AccountType = accountType;
			Status = AccountStatus.Active;
			Balance = 0;
			OpenDate = now;
			CustomerId = customerId;

			CreatedAt = now;
		}

		public void Update(AccountType accountType, AccountStatus status, decimal balance)
		{
			AccountType = accountType;
			Status = status;
			Balance = balance;

			UpdatedAt = DateTime.UtcNow;
		}

		public void Delete()
		{
			IsDeleted = true;
			DeletedAt = DateTime.UtcNow;
		}
	}
}
