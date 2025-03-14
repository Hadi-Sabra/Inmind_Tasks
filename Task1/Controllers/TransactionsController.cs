using Task1.Data;
using Task1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task1.Services;

[Route("api/transactions")]
[ApiController]
public class TransactionsController : ControllerBase
{
    private readonly TransactionDbContext _context;
    private readonly ILanguageService _languageService;

    public TransactionsController(TransactionDbContext context, ILanguageService languageService)
    {
        _context = context;
        _languageService = languageService;
    }

    [HttpPost("notify")]
    public async Task<IActionResult> NotifyTransaction([FromBody] long transactionId)
    {
        var transaction = await _context.Transactions.FindAsync(transactionId);
        if (transaction == null) return NotFound("Transaction not found");

        string language = _languageService.GetLanguage(HttpContext);

        return Ok(new
        {
            Message = $"Transaction Notification: {transaction.GetLocalizedDescription(language)}"
        });
    }
}