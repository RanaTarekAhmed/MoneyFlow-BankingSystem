using MoneyFlow.Data.Entities;
using System.Linq.Expressions;

namespace MoneyFlow.Data.Repositories.Interfaces;

public interface IEmailNotificationRepository
{
    Task<List<EmailNotification>> GetAllAsync(Expression<Func<EmailNotification, bool>>? filter = null);
    Task<EmailNotification?> GetAsync(Expression<Func<EmailNotification, bool>>? filter = null);
    Task AddAsync(EmailNotification emailNotification);
    Task UpdateAsync(EmailNotification emailNotification);
    Task DeleteAsync(int id);
}