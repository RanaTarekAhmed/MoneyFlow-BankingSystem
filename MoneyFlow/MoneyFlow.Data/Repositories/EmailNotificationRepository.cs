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

    public async Task<List<EmailNotification>> GetAllAsync(Expression<Func<EmailNotification, bool>>? filter = null)
    {
        var query = Db.EmailNotifications.Where(e => !e.IsDeleted).AsQueryable();
        if (filter != null)
        {
            query = query.Where(filter);
        }
        return await query.ToListAsync();
    }

    public async Task<EmailNotification?> GetAsync(Expression<Func<EmailNotification, bool>>? filter = null)
    {
        var query = Db.EmailNotifications.Where(e => !e.IsDeleted).AsQueryable();
        if (filter != null)
        {
            return await query.FirstOrDefaultAsync(filter);
        }
        return await query.FirstOrDefaultAsync();
    }

    public async Task AddAsync(EmailNotification emailNotification)
    {
        await Db.EmailNotifications.AddAsync(emailNotification);
        await Db.SaveChangesAsync();
    }

    public async Task UpdateAsync(EmailNotification emailNotification)
    {
        var existingNotification = await Db.EmailNotifications.FirstOrDefaultAsync(e => e.Id == emailNotification.Id && !e.IsDeleted);
        if (existingNotification == null)
        {
            return;
        }
        Db.EmailNotifications.Update(emailNotification);
        await Db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var emailNotification = await Db.EmailNotifications.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        if (emailNotification == null)
        {
            return;
        }
        emailNotification.Delete();
        await Db.SaveChangesAsync();
    }
}