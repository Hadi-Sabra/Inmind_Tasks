using Task1.Data;
using Task1.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Task1.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly TransactionDbContext _context;

        public AccountRepository(TransactionDbContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetByIdAsync(long id)
        {
            return await _context.Accounts.FindAsync(id);
        }

        public async Task UpdateAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }
    }
}