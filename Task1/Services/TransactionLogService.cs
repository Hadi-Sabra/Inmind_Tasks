using Microsoft.EntityFrameworkCore;
using Task1.Data;
using Task1.Models;
using Task1.Services;

namespace Task1.Services;
public class TransactionLogService : ITransactionLogService
{
    private readonly TransactionDbContext _dbContext;
    private readonly ILogger<TransactionLogService> _logger;

    public TransactionLogService(TransactionDbContext dbContext, ILogger<TransactionLogService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<TransactionLog> AddTransactionLogAsync(TransactionLog transactionLog)
    {
        try
        {
            transactionLog.Timestamp = DateTime.UtcNow;
            await _dbContext.TransactionLogs.AddAsync(transactionLog);
            await _dbContext.SaveChangesAsync();
                
            _logger.LogInformation($"Added transaction log with ID: {transactionLog.Id}");
            return transactionLog;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error adding transaction log: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<TransactionLog>> GetTransactionLogsByAccountIdAsync(long accountId)
    {
        try
        {
            var logs = await _dbContext.TransactionLogs
                .Where(t => t.AccountId == accountId)
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();
                
            _logger.LogInformation($"Retrieved {logs.Count} transaction logs for account ID: {accountId}");
            return logs;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving transaction logs: {ex.Message}");
            throw;
        }
    }

    public IQueryable<TransactionLog> GetTransactionLogsQueryable()
    {
        return _dbContext.TransactionLogs.AsQueryable();
    }
    
    public async Task<IEnumerable<TransactionLog>> GetCommonTransactionsAsync(List<long> accountIds)
    {
        // Logic to find common transactions between multiple accounts
        var transactions = await _dbContext.TransactionLogs
            .Where(t => accountIds.Contains(t.AccountId)) // Filter by provided account IDs
            .GroupBy(t => new { t.TransactionType, t.Amount }) // Group by transaction type and amount
            .Where(g => g.Count() > 1) // Only consider common transactions
            .Select(g => g.FirstOrDefault()) // Select the first transaction from each common group
            .ToListAsync();

        return transactions;
    }
    
    public async Task<AccountBalanceSummary> GetAccountBalanceSummaryAsync(long userId)
    {
        // Logic to calculate balance summary for a user based on transactions
        var accountTransactions = await _dbContext.TransactionLogs
            .Where(t => t.AccountId == userId) // Get all transactions for the specific user
            .ToListAsync();

        var totalDeposits = accountTransactions
            .Where(t => t.TransactionType == "Deposit")
            .Sum(t => t.Amount);

        var totalWithdrawals = accountTransactions
            .Where(t => t.TransactionType == "Withdrawal")
            .Sum(t => t.Amount);

        var totalBalance = totalDeposits - totalWithdrawals;

        return new AccountBalanceSummary
        {
            TotalDeposits = totalDeposits,
            TotalWithdrawals = totalWithdrawals,
            TotalBalance = totalBalance
        };
    }


public class AccountBalanceSummary
{
    public decimal TotalDeposits { get; set; }
    public decimal TotalWithdrawals { get; set; }
    public decimal TotalBalance { get; set; }
}
    
}