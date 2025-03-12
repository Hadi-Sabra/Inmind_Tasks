using Task1.Models;

namespace Task1.Services
{
    public interface ILogService
    {
        Task LogAsync(LogEntry logEntry);
        Task<IEnumerable<LogEntry>> GetLogsAsync(string requestId, string routeUrl, DateTime? startDate, DateTime? endDate);
    }
}