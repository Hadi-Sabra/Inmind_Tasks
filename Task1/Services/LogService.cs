using Microsoft.EntityFrameworkCore;
using Task1.Data;
using Task1.Models;
using Microsoft.Extensions.Logging;

namespace Task1.Services
{
    public class LogService : ILogService
    {
        private readonly TransactionDbContext _dbContext;
        private readonly ILogger<LogService> _logger;

        public LogService(TransactionDbContext dbContext, ILogger<LogService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // Method to log data to PostgreSQL database
        public async Task LogAsync(LogEntry logEntry)
        {
            try
            {
                // Add log entry to the database
                await _dbContext.LogEntries.AddAsync(logEntry);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Log entry with RequestId: {logEntry.RequestId} saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while saving log entry: {ex.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<LogEntry>> GetLogsAsync(string requestId, string routeUrl, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var query = _dbContext.LogEntries.AsQueryable();

                if (!string.IsNullOrEmpty(requestId))
                {
                    if (Guid.TryParse(requestId, out Guid parsedRequestId))
                    {
                        query = query.Where(log => log.RequestId == parsedRequestId);
                    }
                    else
                    {
                        _logger.LogError($"Invalid RequestId format: {requestId}");
                        return Enumerable.Empty<LogEntry>();  // Return empty list if invalid
                    }
                }

                if (!string.IsNullOrEmpty(routeUrl))
                    query = query.Where(log => log.RouteUrl.Contains(routeUrl));

                if (startDate.HasValue)
                    query = query.Where(log => log.Timestamp >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(log => log.Timestamp <= endDate.Value);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while fetching logs: {ex.Message}");
                throw;
            }
        }

    }
}
