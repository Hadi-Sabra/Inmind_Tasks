using Task1.Models;
using System.Threading.Tasks;

namespace Task1.Repositories
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);
    }
}