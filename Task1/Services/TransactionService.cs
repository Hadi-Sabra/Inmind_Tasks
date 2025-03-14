using Task1.DTOs;
using Task1.Repositories;
using Task1.Models;
using System;
using System.Threading.Tasks;

namespace Task1.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TransferResponseDto> TransferFundsAsync(TransferRequestDto request)
        {
            var fromAccount = await _unitOfWork.Accounts.GetByIdAsync(request.FromAccountId);
            var toAccount = await _unitOfWork.Accounts.GetByIdAsync(request.ToAccountId);

            if (fromAccount == null || toAccount == null)
                return new TransferResponseDto { Message = "Invalid account(s).", Success = false };

            if (fromAccount.Balance < request.Amount)
                return new TransferResponseDto { Message = "Insufficient funds.", Success = false };

            // Perform transaction
            fromAccount.Balance -= request.Amount;
            toAccount.Balance += request.Amount;

            var transaction = new Transaction
            {
                FromAccountId = request.FromAccountId,
                ToAccountId = request.ToAccountId,
                Amount = request.Amount
            };

            await _unitOfWork.Transactions.AddAsync(transaction);
            await _unitOfWork.CompleteAsync();

            return new TransferResponseDto { Message = "Transfer successful.", Success = true };
        }
    }
}