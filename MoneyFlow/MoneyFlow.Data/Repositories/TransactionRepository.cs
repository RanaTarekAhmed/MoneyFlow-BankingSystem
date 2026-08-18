using MoneyFlow.Data.Entities;
using MoneyFlow.Data.Enums;
using MoneyFlow.Data.Repositories.Interfaces;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using MoneyFlow.Data.Database;



namespace MoneyFlow.Data.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly MoneyFlowDbContext dp_context;

        public TransactionRepository(MoneyFlowDbContext DP)
        {
            dp_context = DP;
        }
        public async Task AddAsync(Transaction transaction)
        {

            await dp_context.Transactions.AddAsync(transaction);
            await dp_context.SaveChangesAsync();

        }

        public async Task<List<Transaction>> GetAllAsync(Expression<Func<Transaction, bool>>? filter)
        {
            var query = dp_context.Transactions.Where(t => (t.SenderAccount == null || !t.SenderAccount.IsDeleted) &&(t.ReceiverAccount == null || !t.ReceiverAccount.IsDeleted));
            if (filter != null)
            {
               query=query.Where(filter);
            }

            return await query.ToListAsync();

        }

        public async Task<Transaction?> GetAsync(Expression<Func<Transaction, bool>> filter)
        {
            var query = dp_context.Transactions.Where(t => (t.SenderAccount == null || !t.SenderAccount.IsDeleted) && (t.ReceiverAccount == null || !t.ReceiverAccount.IsDeleted));
            return await query.Where(filter).FirstOrDefaultAsync();

        }

        public async Task UpdateStatusAsync(int id, TransactionStatus status)
        {
            var transaction = await dp_context.Transactions.Where(t => (t.SenderAccount == null || !t.SenderAccount.IsDeleted) && (t.ReceiverAccount == null || !t.ReceiverAccount.IsDeleted)).FirstOrDefaultAsync(x => x.Id == id);

            if (transaction != null)
            {
                transaction.UpdateStatus(status);

                await dp_context.SaveChangesAsync();
            }

        }
    }
}
