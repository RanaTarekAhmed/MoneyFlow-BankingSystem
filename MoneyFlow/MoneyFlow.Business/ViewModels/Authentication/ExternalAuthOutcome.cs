using MoneyFlow.Data.Entities;


namespace MoneyFlow.Business.ViewModels.Authentication
{
	public class ExternalAuthOutcome
	{
		public bool Succeeded { get; init; }
		public bool RequiresRegistration { get; init; }
		public ApplicationUser? User { get; init; }
		public string? ErrorMessage { get; init; }

		public static ExternalAuthOutcome Success(ApplicationUser user) =>
			new() { Succeeded = true, User = user };

		public static ExternalAuthOutcome RegistrationRequired() =>
			new() { RequiresRegistration = true };

		public static ExternalAuthOutcome Failed(string errorMessage) =>
			new() { ErrorMessage = errorMessage };
	}
}
