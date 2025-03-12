using Microsoft.AspNetCore.Mvc;
using Task1.DTOs;
using Task1.Models;
using Task1.Services;

namespace Task1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionEventsController : ControllerBase
    {
        private readonly TransactionEventService _transactionEventService;

        public TransactionEventsController(TransactionEventService transactionEventService)
        {
            _transactionEventService = transactionEventService;
        }

        [HttpPost]
        [Route("events")]
        public async Task<IActionResult> DispatchEvent([FromBody] TransactionEventsDto eventRequest)
        {
            if (eventRequest == null)
            {
                return BadRequest("Invalid event request.");
            }

            var result = await _transactionEventService.DispatchTransactionEventAsync(
                eventRequest.TransactionId,
                eventRequest.EventType,
                eventRequest.Details
            );

            if (!result)
            {
                return StatusCode(500, "Error dispatching event.");
            }

            return Ok("Event dispatched successfully.");
        }

        [HttpGet]
        [Route("events/{transactionId}")]
        public async Task<IActionResult> GetEventsByTransactionId(long transactionId)
        {
            var events = await _transactionEventService.GetEventsByTransactionIdAsync(transactionId);
            if (events == null || !events.Any())
            {
                return NotFound("No events found for this transaction.");
            }

            return Ok(events);
        }
    }
}