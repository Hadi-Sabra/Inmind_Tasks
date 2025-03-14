using Task1.DTOs;
using System.Threading.Tasks;

namespace Task1.Services
{
    public interface ITransactionService
    {
        Task<TransferResponseDto> TransferFundsAsync(TransferRequestDto request);
    }
}