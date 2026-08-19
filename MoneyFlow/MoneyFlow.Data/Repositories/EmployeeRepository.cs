using Microsoft.EntityFrameworkCore;
using MoneyFlow.Data.Database;
using MoneyFlow.Data.Entities;
using MoneyFlow.Data.Repositories.Interfaces;
using System.Linq.Expressions;

namespace MoneyFlow.Data.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly MoneyFlowDbContext Db;
        public EmployeeRepository(MoneyFlowDbContext db)
        {
            Db = db;
        }
        
        public async Task<List<Employee>> GetAllAsync(Expression<Func<Employee, bool>>? filter = null)
        {
            var query = Db.Employees.Where(e => !e.IsDeleted);
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.ToListAsync();
        }

        public async Task<Employee?> GetAsync(Expression<Func<Employee, bool>>? filter = null)
        {
            var query = Db.Employees.Where(e => !e.IsDeleted);
            if (filter != null)
            {
                return await query.FirstOrDefaultAsync(filter);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task AddAsync(Employee employee)
        {
            await Db.Employees.AddAsync(employee);
            await Db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Employee employee)
        {
            var oldEmployee = await Db.Employees.FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == employee.Id);
            if (oldEmployee == null)
            {
                return;
            }
            oldEmployee.Update(employee.Salary, employee.HireDate);
            await Db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var employee = await Db.Employees.FirstOrDefaultAsync(e => !e.IsDeleted && e.Id == id);

            if (employee == null)
            {
                return;
            }
            employee.Delete();
            await Db.SaveChangesAsync();
        }
    }
}