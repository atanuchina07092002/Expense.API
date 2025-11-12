using ExpenseTracker.Dtos;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Models;
using System.Linq;
namespace ExpenseTracker.Data.Services
{
    public interface ITransactionServices {
        public  Task CreateTransaction(TransactionDto tran, int userId);
        public Task<List<Transaction>>? GetAllTransaction(int userId);
        public Task<Transaction>? GetTransactionById(int transactionId);
        public Task<Transaction>? UpdateTransaction(int id, TransactionDto transaction);
        public Task<Transaction>? DeleteTransaction(int id);

    }
    public class TransactionServices(AppDbContext _context) : ITransactionServices
    {
        public async Task CreateTransaction(TransactionDto tran, int userId)
        {
            Transaction transaction = new Transaction
            {

                Type = tran.Type,
                Amount = tran.Amount,
                Category = tran.Category,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                UserId = userId
            };
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
           
        }

        public async Task<Transaction>? DeleteTransaction(int id)
        {
            var tran = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id);
            if (tran != null)
            {
               _context.Transactions.Remove(tran);
                await _context.SaveChangesAsync();
            }
            return tran;
        }

        public async Task<List<Transaction>>? GetAllTransaction(int userId)
        {
            var AllTransactions = await _context.Transactions.Where(x=>x.UserId == userId).ToListAsync();
            return AllTransactions;
        }

        public async Task<Transaction>? GetTransactionById(int transactionId)
        {
            var tran = await _context.Transactions.FirstOrDefaultAsync(tn => tn.Id == transactionId);
            return tran;
        }

        public async Task<Transaction>? UpdateTransaction(int id, TransactionDto transaction)
        {
            var tran = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == id);
            if (tran != null)
            {  
                tran.Amount = transaction.Amount;
                tran.Category = transaction.Category;
                tran.Type = transaction.Type;
                tran.CreatedAt = DateTime.UtcNow;

                _context.Transactions.Update(tran);
                await _context.SaveChangesAsync();
            }
            return tran;
           
        }
    }
}
