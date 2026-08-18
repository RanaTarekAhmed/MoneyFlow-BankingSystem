

namespace MoneyFlow.Data.Entities
{
    public class Employee : AuditableEntity
    {
        public int Id { get; private set; }
        public decimal Salary { get; private set; }
        public DateOnly HireDate { get; private set; }
        public string UserId { get; private set; } = string.Empty;
        public ApplicationUser User { get; private set; } = null!;
        public List<Transaction> Transactions { get; private set; } = new List<Transaction>();

        private Employee()
        {
            // For EF Core
        }

        public Employee(decimal salary, DateOnly hireDate, string userId)
        {
            Salary = salary;
            HireDate = hireDate;
            UserId = userId;

            CreatedAt = DateTime.UtcNow;
        }

        public void Update(decimal salary, DateOnly hireDate)
        {
            Salary = salary;
            HireDate = hireDate;

            UpdatedAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }
    }
}
