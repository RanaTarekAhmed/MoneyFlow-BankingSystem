using MoneyFlow.Business.ViewModels.Accounts;
using System;
using System.Collections.Generic;
using System.Text;
using MoneyFlow.Data.Entities;

namespace MoneyFlow.Business.Services.Interfaces
{
    public interface IAccountService
    {
        Task<MyAccountsVM?> GetMyAccountsAsync(string userId);

        Task<AccountDetailsVM?> GetAccountDetailsAsync(string userId, int accountId);

        Task<TransferVM?> GetTransferModelAsync(string userId);

        Task<(bool Success, string Message, Transaction? Transaction)> TransferAsync(string userId, TransferVM model);
    }
}
