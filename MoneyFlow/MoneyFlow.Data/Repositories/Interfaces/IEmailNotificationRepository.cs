using MoneyFlow.Data.Entities;
using System.Linq.Expressions;

namespace MoneyFlow.Data.Repositories.Interfaces;

public interface IEmailNotificationRepository
{
    List<EmailNotification> GetAll(Expression<Func<EmailNotification, bool>>? filter = null);
    EmailNotification Get(Expression<Func<EmailNotification, bool>>? filter = null);
    bool Add(EmailNotification emailNotification);
    bool Update(EmailNotification emailNotification);
    bool Delete(int id);
}