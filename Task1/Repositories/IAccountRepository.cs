using Task1.Models;
using System.Threading.Tasks;

namespace Task1.Repositories
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(long id);
        Task UpdateAsync(Account account);
    }
}