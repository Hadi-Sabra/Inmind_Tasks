using MediatR;
using Microsoft.EntityFrameworkCore;
using Task1.Data;
using Task1.Models;
using Task1.Services;

namespace Task1.Services
{
    public class TransactionEventService
    {
        private readonly TransactionDbContext _dbContext;
        private readonly IMediator _mediator;

        public TransactionEventService(TransactionDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }

        public async Task<bool> DispatchTransactionEventAsync(long transactionId, string eventType, string details)
        {
            var transactionEvent = new TransactionEvent
            {
                TransactionId = transactionId,
                EventType = eventType,
                Details = details
            };

            // Save event to the database
            _dbContext.TransactionEvents.Add(transactionEvent);
            await _dbContext.SaveChangesAsync();

            // Dispatch the event using MediatR
            await _mediator.Send(new DispatchTransactionEventRequest
            {
                TransactionId = transactionId,
                EventType = eventType,
                Details = details
            });

            return true;
        }

        public async Task<List<TransactionEvent>> GetEventsByTransactionIdAsync(long transactionId)
        {
            return await _dbContext.TransactionEvents
                .Where(e => e.TransactionId == transactionId)
                .ToListAsync();
        }
    }
}