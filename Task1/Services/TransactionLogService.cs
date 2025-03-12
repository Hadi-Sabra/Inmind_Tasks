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
}