

namespace MoneyFlow.Data.Entities
{
    public class Customer : AuditableEntity
    {
        public int Id { get; private set; }
        public string NationalId { get; private set; } = string.Empty;
        public string UserId { get; private set; } = string.Empty;
        public ApplicationUser User { get; private set; } = null!;
        public List<Account> Accounts { get; private set; } = new List<Account>();

        private Customer()
        {
            // For EF Core
        }

        public Customer(string nationalId, string userId)
        {
            NationalId = nationalId;
            UserId = userId;

            CreatedAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }
    }
}
