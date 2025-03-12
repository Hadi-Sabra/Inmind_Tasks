using Task1.Models;

namespace Task1.Services;

public class ITransactionLogService
{
    public Task<TransactionLog> AddTransactionLogAsync(TransactionLog transactionLog)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TransactionLog>> GetTransactionLogsByAccountIdAsync(long accountId)
    {
        throw new NotImplementedException();
    }

    public IQueryable<TransactionLog> GetTransactionLogsQueryable()
    {
        throw new NotImplementedException();
    }
}