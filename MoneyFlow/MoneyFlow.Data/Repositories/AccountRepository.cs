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

        public async Task<(List<Account> Items, int TotalCount)> GetAllAccountsPagedAsync(int pageNumber, int pageSize, Expression<Func<Account, bool>>? filter)
        {
            IQueryable<Account> query = _context.Accounts;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Include(a => a.Customer)
                    .ThenInclude(c => c.User)
                .OrderByDescending(a => a.OpenDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> AnyAsync(Expression<Func<Account, bool>>? filter)
        {
            if (filter != null)
            {
                return await _context.Accounts.AnyAsync(filter);
            }

            return await _context.Accounts.AnyAsync();
        }
    }
}