using MoneyFlow.Business.ViewModels.Accounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoneyFlow.Business.Services.Interfaces
{
    public interface IAccountService
    {
        Task<MyAccountsVM?> GetMyAccountsAsync(string userId);

        Task<AccountDetailsVM?> GetAccountDetailsAsync(string userId, int accountId);
    }
}
