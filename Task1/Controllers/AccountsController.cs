using Task1.DTOs;
using Task1.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Task1.Data;

namespace Task1.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILanguageService _languageService;
        private readonly TransactionDbContext _context;

        public AccountsController(ITransactionService service, ILanguageService languageService,TransactionDbContext context)
        {
            _transactionService = service;
            _languageService = languageService;
            _context = context;
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> TransferFunds([FromBody] TransferRequestDto request)
        {
            var response = await _transactionService.TransferFundsAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }
        
        [HttpGet("{accountId}/details")]
        public async Task<IActionResult> GetAccountDetails(long accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null) return NotFound("Account not found");

            string language = _languageService.GetLanguage(HttpContext);
            return Ok(new
            {
                Id = account.Id,
                Name = account.GetLocalizedName(language),
                Balance = account.Balance
            });
        }
    }
}