using System.Linq.Expressions;
using MoneyFlow.Data.Entities;

namespace MoneyFlow.Data.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        Task<List<Account>> GetAllAsync(Expression<Func<Account, bool>>? filter = null);
        Task<Account?> GetAsync(Expression<Func<Account, bool>> filter);
        Task<(List<Account> Items, int TotalCount)> GetAllAccountsPagedAsync
            (
            int pageNumber,
            int pageSize,
            Expression<Func<Account, bool>>? filter
            );
        Task AddAsync(Account account);
        Task UpdateAsync(Account account);
        Task DeleteAsync(int id);
    }
}
