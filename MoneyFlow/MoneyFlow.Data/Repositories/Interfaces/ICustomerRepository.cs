using MoneyFlow.Data.Entities;
using System.Linq.Expressions;


namespace MoneyFlow.Data.Repositories.Interfaces
{
	public interface ICustomerRepository
	{
		Task<List<Customer>> GetAllAsync(Expression<Func<Customer, bool>>? filter);
		Task<Customer?> GetAsync(Expression<Func<Customer, bool>>? filter);
		Task AddAsync(Customer customer);
		Task DeleteAsync(int id);

        Task<(List<Customer> Customers, int TotalCount)> GetPagedAsync(int pageNumber,int pageSize,Expression<Func<Customer, bool>>? filter);
    }
}
