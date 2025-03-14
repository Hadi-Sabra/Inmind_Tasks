using System.Threading.Tasks;

namespace Task1.Repositories
{
    public interface IUnitOfWork
    {
        IAccountRepository Accounts { get; }
        ITransactionRepository Transactions { get; }
        Task<int> CompleteAsync();
    }
}