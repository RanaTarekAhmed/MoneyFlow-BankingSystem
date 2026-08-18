using Microsoft.EntityFrameworkCore;
using MoneyFlow.Data.Database;
using MoneyFlow.Data.Entities;
using MoneyFlow.Data.Repositories.Interfaces;
using System.Linq.Expressions;


namespace MoneyFlow.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly MoneyFlowDbContext _context;

        public CustomerRepository(MoneyFlowDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => !c.IsDeleted && c.Id == id);

            if (customer == null)
            {
                return;
            }

            customer.Delete();

            await _context.SaveChangesAsync();
        }

        public async Task<Customer?> GetAsync(Expression<Func<Customer, bool>>? filter)
        {
            var query = _context.Customers.Where(c => !c.IsDeleted);

            if (filter != null)
            {
                return await query.FirstOrDefaultAsync(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<Customer>> GetAllAsync(Expression<Func<Customer, bool>>? filter)
        {
            var query = _context.Customers.Where(c => !c.IsDeleted);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }
    }
}
