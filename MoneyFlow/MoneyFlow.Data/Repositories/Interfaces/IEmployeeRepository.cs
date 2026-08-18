using MoneyFlow.Data.Entities;
using System.Linq.Expressions;

namespace MoneyFlow.Data.Repositories.Interfaces;

public interface IEmployeeRepository
{
    List<Employee> GetAll(Expression<Func<Employee, bool>>? filter = null);
    Employee Get(Expression<Func<Employee, bool>>? filter = null);
    bool Add(Employee employee);
    bool Update(Employee employee);
    bool Delete(int id);
}