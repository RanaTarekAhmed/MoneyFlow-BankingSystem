using Microsoft.AspNetCore.Identity;


namespace MoneyFlow.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateOnly? DateOfBirth { get; set; }
        public Customer? Customer { get; set; }
        public Employee? Employee { get; set; }
        public List<EmailNotification> EmailNotifications { get; set; } = new List<EmailNotification>();
    }
}
