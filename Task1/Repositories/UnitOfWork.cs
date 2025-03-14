using Task1.Data;
using System.Threading.Tasks;

namespace Task1.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TransactionDbContext _context;
        public IAccountRepository Accounts { get; }
        public ITransactionRepository Transactions { get; }

        public UnitOfWork(TransactionDbContext context, IAccountRepository accounts, ITransactionRepository transactions)
        {
            _context = context;
            Accounts = accounts;
            Transactions = transactions;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}