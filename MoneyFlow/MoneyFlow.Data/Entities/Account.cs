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

		public void Update(AccountType accountType, AccountStatus status/*, decimal balance*/)
		{
			AccountType = accountType;
			Status = status;
			//Balance = balance;
			UpdatedAt = DateTime.UtcNow;
		}

		public void Delete()
		{
			IsDeleted = true;
			DeletedAt = DateTime.UtcNow;
		}



		//To be used in SERVICE layer
		/*
        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Deposit amount must be greater than zero.");

            Balance += amount;
            UpdatedAt = DateTime.Now;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Withdrawal amount must be greater than zero.");

            if (amount > Balance)
                throw new InvalidOperationException("Insufficient balance.");

            Balance -= amount;
            UpdatedAt = DateTime.Now;
        }

        public void Transfer(Account destinationAccount, decimal amount)
        {
            if (destinationAccount == null)
                throw new ArgumentNullException(nameof(destinationAccount));

            if (destinationAccount == this)
                throw new InvalidOperationException("Cannot transfer money to the same account.");

            Withdraw(amount);
            destinationAccount.Deposit(amount);
        }*/
    }
}
