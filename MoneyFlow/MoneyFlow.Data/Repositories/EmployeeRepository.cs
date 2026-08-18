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
        
        public List<Employee> GetAll(Expression<Func<Employee, bool>>? filter = null)
        {
            try
            {
                if (filter != null)
                {
                    var result = Db.Employees.Where(filter).ToList();
                    return result;
                }
                else
                {
                    var result = Db.Employees.ToList();
                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Employee Get(Expression<Func<Employee, bool>>? filter = null)
        {
            try
            {
                if (filter != null)
                {
                    var result = Db.Employees.Where(filter).FirstOrDefault();
                    return result;
                }
                else
                {
                    var result = Db.Employees.FirstOrDefault();
                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool Add(Employee employee)
        {
            try
            {
                var result = Db.Employees.Add(employee);
                Db.SaveChanges();
                if (result.Entity.Id > 0)
                    return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Update(Employee employee)
        {
            try
            {
                var oldEmployee = Db.Employees.FirstOrDefault(e => e.Id == employee.Id);

                if (oldEmployee == null)
                    return false;

                oldEmployee.Update(
                    employee.Salary,
                    employee.HireDate
                );

                Db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Delete(int id)
        {
            try
            {
                var employee = Db.Employees.FirstOrDefault(e => e.Id == id);
                if (employee == null) return false;
                employee.Delete();
                Db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

