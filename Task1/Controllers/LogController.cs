using Microsoft.AspNetCore.Mvc;
using Task1.Models;
using Task1.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Task1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logEntryService;

        public LogController(ILogService logEntryService)
        {
            _logEntryService = logEntryService;
        }

        // POST: api/logs
        [HttpPost]
        public async Task<IActionResult> LogEvent([FromBody] LogEntry logEntry)
        {
            try
            {
                if (logEntry == null)
                {
                    return BadRequest("Log entry is required.");
                }

                // Assuming logs are sent via RabbitMQ or any other service.
                await _logEntryService.LogAsync(logEntry);

                return Ok(new { Message = "Log entry added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/logs
        [HttpGet]
        public async Task<IActionResult> GetLogs([FromQuery] string requestId, [FromQuery] string routeUrl, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var logs = await _logEntryService.GetLogsAsync(requestId, routeUrl, startDate, endDate);
                if (logs == null)
                {
                    return NotFound("No logs found matching the criteria.");
                }

                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}