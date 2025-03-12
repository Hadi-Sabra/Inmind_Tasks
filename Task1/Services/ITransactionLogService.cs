using Task1.Models;

namespace Task1.Services;

public interface ITransactionLogService
{
    public Task<TransactionLog> AddTransactionLogAsync(TransactionLog transactionLog);


    public Task<IEnumerable<TransactionLog>> GetTransactionLogsByAccountIdAsync(long accountId);


    public IQueryable<TransactionLog> GetTransactionLogsQueryable();

    Task<TransactionLogService.AccountBalanceSummary> GetAccountBalanceSummaryAsync(long userId);

    Task<IEnumerable<TransactionLog>> GetCommonTransactionsAsync(List<long> accountIds);
}