using Microsoft.EntityFrameworkCore;
using MoneyFlow.Data.Database;
using MoneyFlow.Data.Entities;
using MoneyFlow.Data.Repositories.Interfaces;
using System.Linq.Expressions;

namespace MoneyFlow.Data.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly MoneyFlowDbContext _context;

        public AccountRepository(MoneyFlowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Account account)
        {
            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => !a.IsDeleted && a.Id == id);
            if (account == null)
            {
                return;
            }
            account.Delete();
            await _context.SaveChangesAsync();
        }

        public async Task<Account?> GetAsync(Expression<Func<Account, bool>>? filter)
        {
            var query = _context.Accounts.Where(a => !a.IsDeleted);
            if (filter != null)
            {
                return await query.FirstOrDefaultAsync(filter);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<Account>> GetAllAsync(Expression<Func<Account, bool>>? filter)
        {
            var query = _context.Accounts.Where(a => !a.IsDeleted);

            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.ToListAsync();
        }

        public async Task UpdateAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }
    }
}