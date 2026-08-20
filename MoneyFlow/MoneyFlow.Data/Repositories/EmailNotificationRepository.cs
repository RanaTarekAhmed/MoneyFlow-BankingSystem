using Microsoft.EntityFrameworkCore;
using MoneyFlow.Data.Database;
using MoneyFlow.Data.Entities;
using MoneyFlow.Data.Repositories.Interfaces;
using System.Linq.Expressions;

namespace MoneyFlow.Data.Repositories;

public class EmailNotificationRepository : IEmailNotificationRepository
{
    private readonly MoneyFlowDbContext _context;

    public EmailNotificationRepository(MoneyFlowDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmailNotification>> GetAllAsync(Expression<Func<EmailNotification, bool>>? filter = null)
    {
        var query = _context.EmailNotifications.Where(e => !e.IsDeleted).AsQueryable();
        if (filter != null)
        {
            query = query.Where(filter);
        }
        return await query.ToListAsync();
    }

    public async Task<EmailNotification?> GetAsync(Expression<Func<EmailNotification, bool>>? filter = null)
    {
        var query = _context.EmailNotifications.Where(e => !e.IsDeleted).AsQueryable();
        if (filter != null)
        {
            return await query.FirstOrDefaultAsync(filter);
        }
        return await query.FirstOrDefaultAsync();
    }

    public async Task AddAsync(EmailNotification emailNotification)
    {
        await _context.EmailNotifications.AddAsync(emailNotification);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(EmailNotification emailNotification)
    {
        var existingNotification = await _context.EmailNotifications.FirstOrDefaultAsync(e => e.Id == emailNotification.Id && !e.IsDeleted);
        if (existingNotification == null)
        {
            return;
        }
        _context.EmailNotifications.Update(emailNotification);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var emailNotification = await _context.EmailNotifications.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        if (emailNotification == null)
        {
            return;
        }
        emailNotification.Delete();
        await _context.SaveChangesAsync();
    }
}