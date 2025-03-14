using Task1.DTOs;
using Task1.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Task1.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public AccountsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> TransferFunds([FromBody] TransferRequestDto request)
        {
            var response = await _transactionService.TransferFundsAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }
    }
}