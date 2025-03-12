using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Task1.DTOs;
using Task1.Models;
using Task1.Services;
using Task1.Messaging;

namespace Task1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionLogsController : ControllerBase
    {
        private readonly ITransactionLogService _transactionLogService;
        private readonly RabbitMQService _rabbitMQService;
        private readonly ILogger<TransactionLogsController> _logger;

        public TransactionLogsController(
            ITransactionLogService transactionLogService,
            RabbitMQService rabbitMQService,
            ILogger<TransactionLogsController> logger)
        {
            _transactionLogService = transactionLogService;
            _rabbitMQService = rabbitMQService;
            _logger = logger;
        }

        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TransactionLogDto>> CreateTransactionLog(CreateTransactionLogDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var transactionLog = new TransactionLog
            {
                AccountId = createDto.AccountId,
                TransactionType = createDto.TransactionType,
                Amount = createDto.Amount,
                Status = createDto.Status,
                Details = createDto.Details,
                Timestamp = DateTime.UtcNow
            };

            try
            {
                var createdLog = await _transactionLogService.AddTransactionLogAsync(transactionLog);
                
                
                _rabbitMQService.PublishTransactionLog(createdLog);
                
                var resultDto = new TransactionLogDto
                {
                    Id = createdLog.Id,
                    AccountId = createdLog.AccountId,
                    TransactionType = createdLog.TransactionType,
                    Amount = createdLog.Amount,
                    Timestamp = createdLog.Timestamp,
                    Status = createdLog.Status,
                    Details = createdLog.Details
                };
                
                _logger.LogInformation($"Created transaction log: {resultDto.Id}");
                return CreatedAtAction(nameof(GetTransactionLogsByAccountId), new { accountId = resultDto.AccountId }, resultDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating transaction log: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating transaction log");
            }
        }

        // GET /transaction-logs/{accountId}
        [HttpGet("{accountId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<TransactionLogDto>>> GetTransactionLogsByAccountId(long accountId)
        {
            try
            {
                var logs = await _transactionLogService.GetTransactionLogsByAccountIdAsync(accountId);
                
                if (!logs.Any())
                {
                    return NotFound($"No transaction logs found for account ID: {accountId}");
                }
                
                var logDtos = logs.Select(log => new TransactionLogDto
                {
                    Id = log.Id,
                    AccountId = log.AccountId,
                    TransactionType = log.TransactionType,
                    Amount = log.Amount,
                    Timestamp = log.Timestamp,
                    Status = log.Status,
                    Details = log.Details
                });
                
                _logger.LogInformation($"Retrieved {logDtos.Count()} transaction logs for account ID: {accountId}");
                return Ok(logDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving transaction logs: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving transaction logs");
            }
        }

        // GET /transaction-logs (OData)
        [HttpGet]
        [EnableQuery]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetTransactionLogs()
        {
            try
            {
                var logs = _transactionLogService.GetTransactionLogsQueryable()
                    .Select(log => new TransactionLogDto
                    {
                        Id = log.Id,
                        AccountId = log.AccountId,
                        TransactionType = log.TransactionType,
                        Amount = log.Amount,
                        Timestamp = log.Timestamp,
                        Status = log.Status,
                        Details = log.Details
                    });
                
                _logger.LogInformation("Queried transaction logs with OData");
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error querying transaction logs: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error querying transaction logs");
            }
        }
    }
}