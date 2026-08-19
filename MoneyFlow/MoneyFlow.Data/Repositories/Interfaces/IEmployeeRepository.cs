using MoneyFlow.Data.Entities;
using System.Linq.Expressions;

namespace MoneyFlow.Data.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAllAsync(Expression<Func<Employee, bool>>? filter = null);
        Task<Employee?> GetAsync(Expression<Func<Employee, bool>>? filter = null);
        Task AddAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(int id);
    }
}