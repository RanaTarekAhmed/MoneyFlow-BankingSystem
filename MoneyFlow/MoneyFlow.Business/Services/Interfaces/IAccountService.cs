using MoneyFlow.Business.ViewModels.Accounts;
using MoneyFlow.Data.Entities;
using MoneyFlow.Business.Common;

namespace MoneyFlow.Business.Services.Interfaces
{
    public interface IAccountService
    {
        Task<MyAccountsVM?> GetMyAccountsAsync(string userId);
        Task<AccountDetailsVM?> GetAccountDetailsAsync(string userId, int accountId);
        Task<AccountDetailsVM?> GetAccountByNumberAsync(string accountNumber);
        Task<TransferVM?> GetTransferModelAsync(string userId);
        Task<PagedResult<EmployeeAccountVM>> GetAllAccountsPagedAsync
            (
            int pageNumber,
            int pageSize,
            AccountQueryVM? query
            );
        Task<EmployeeAccountSummaryVM> GetAllAccountsSummaryAsync();
        Task<(bool Success, string Message, Transaction? Transaction)> TransferAsync(string userId, TransferVM model);

        Task<(bool Success, string Message, Transaction? Transaction)> DepositAsync(CashOperationVM model);

        Task<(bool Success, string Message, Transaction? Transaction)> WithdrawAsync(CashOperationVM model);
    }
}
