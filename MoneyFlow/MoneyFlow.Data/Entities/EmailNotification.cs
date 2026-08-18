using MoneyFlow.Data.Enums;


namespace MoneyFlow.Data.Entities
{
	public class EmailNotification : AuditableEntity
	{
		public int Id { get; private set; }
		public string ToEmail { get; private set; } = string.Empty;
		public string Subject { get; private set; } = string.Empty;
		public string Body { get; private set; } = string.Empty;
		public EmailNotificationStatus Status { get; private set; }
		public DateTime? SentAt { get; private set; }
		public string? ErrorMessage { get; private set; }
		public string UserId { get; private set; } = string.Empty;
		public ApplicationUser User { get; private set; } = null!;

		private EmailNotification()
		{
			// For EF Core
		}

		public EmailNotification(string toEmail, string subject, string body, string userId)
		{
			ToEmail = toEmail;
			Subject = subject;
			Body = body;
			Status = EmailNotificationStatus.Pending;
			SentAt = null;
			ErrorMessage = null;
			UserId = userId;

			CreatedAt = DateTime.UtcNow;
		}

        public void MarkAsSent()
        {
            var now = DateTime.UtcNow;

            Status = EmailNotificationStatus.Sent;
            SentAt = now;
            ErrorMessage = null;

			UpdatedAt = now;
        }

        public void MarkAsFailed(string? errorMessage)
        {
            Status = EmailNotificationStatus.Failed;
            ErrorMessage = errorMessage;


            UpdatedAt = DateTime.UtcNow;
        }

		public void Delete()
		{
			IsDeleted = true;
			DeletedAt = DateTime.UtcNow;
        }
    }
}
