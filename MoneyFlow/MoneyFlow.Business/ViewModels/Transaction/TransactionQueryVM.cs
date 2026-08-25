using MoneyFlow.Data.Enums;


namespace MoneyFlow.Business.ViewModels.Transaction
{
	public class TransactionQueryVM
	{
		public string? Search { get; set; }
		public TransactionType? TransactionType { get; set; }
		public TransactionStatus? Status { get; set; }
	}
}
