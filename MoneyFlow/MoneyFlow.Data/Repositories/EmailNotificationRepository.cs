using Microsoft.EntityFrameworkCore;
using MoneyFlow.Data.Database;
using MoneyFlow.Data.Entities;
using MoneyFlow.Data.Repositories.Interfaces;
using System.Linq.Expressions;

namespace MoneyFlow.Data.Repositories;

public class EmailNotificationRepository : IEmailNotificationRepository
{
    private readonly MoneyFlowDbContext Db;

    public EmailNotificationRepository(MoneyFlowDbContext db)
    {
        Db = db;
    }

    public List<EmailNotification> GetAll(Expression<Func<EmailNotification, bool>>? filter = null)
    {
        try
        {
            if (filter != null)
            {
                var result = Db.EmailNotifications.Where(filter).ToList();
                return result;
            }
            else
            {
                var result = Db.EmailNotifications.ToList();
                return result;
            }
        }
        catch (Exception)
        {
            return null;
        }
    }

    public EmailNotification Get(Expression<Func<EmailNotification, bool>>? filter = null)
    {
        try
        {
            if (filter != null)
            {
                var result = Db.EmailNotifications.Where(filter).FirstOrDefault();
                return result;
            }
            else
            {
                var result = Db.EmailNotifications.FirstOrDefault();
                return result;
            }
        }
        catch (Exception)
        {
            return null;
        }
    }

    public bool Add(EmailNotification emailNotification)
    {
        try
        {
            var result = Db.EmailNotifications.Add(emailNotification);
            Db.SaveChanges();
            if (result.Entity.Id > 0) return true;
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool Update(EmailNotification emailNotification)
    {
        try
        {
            Db.EmailNotifications.Update(emailNotification);
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
            var emailNotification = Db.EmailNotifications.FirstOrDefault(e => e.Id == id);
            if (emailNotification == null) return false;
            emailNotification.Delete();
            Db.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}