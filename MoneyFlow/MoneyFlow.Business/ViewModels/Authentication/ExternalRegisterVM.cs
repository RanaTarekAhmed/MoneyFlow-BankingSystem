using System.ComponentModel.DataAnnotations;


namespace MoneyFlow.Business.ViewModels.Authentication
{
	public class ExternalRegisterVM
	{
		[Required(ErrorMessage = "First name is required.")]
		[StringLength(50, MinimumLength = 2,
			ErrorMessage = "First name must be between 2 and 50 characters.")]
		public string FirstName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Last name is required.")]
		[StringLength(50, MinimumLength = 2,
			ErrorMessage = "Last name must be between 2 and 50 characters.")]
		public string LastName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Date of birth is required.")]
		public DateOnly? DateOfBirth { get; set; }

		[Required(ErrorMessage = "National ID is required.")]
		[RegularExpression(@"^\d{14}$",
			ErrorMessage = "National ID must contain exactly 14 digits.")]
		public string NationalId { get; set; } = string.Empty;

		[Required(ErrorMessage = "Email address is required.")]
		[EmailAddress]
		[StringLength(100, ErrorMessage = "Email address cannot exceed 100 characters.")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "Address is required.")]
		[StringLength(200, MinimumLength = 5,
			ErrorMessage = "Address must be between 5 and 200 characters.")]
		public string Address { get; set; } = string.Empty;
	}
}
