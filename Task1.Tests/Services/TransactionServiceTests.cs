using Xunit;
using Moq;
using Task1.Services;
using Task1.Repositories;
using Task1.Models;
using System.Threading.Tasks;

public class TransactionServiceTests
{
    private readonly Mock<ITransactionRepository> _mockRepo;
    private readonly TransactionService _service;

    public TransactionServiceTests()
    {
        _mockRepo = new Mock<ITransactionRepository>();
        _service = new TransactionService(_mockRepo.Object);
    }

    [Fact]
    public async Task DepositAsync_ShouldIncreaseBalance()
    {
        
        var account = new Account { Id = 1, Balance = 1000 };
        _mockRepo.Setup(r => r.GetAccountByIdAsync(1)).ReturnsAsync(account);

        
        await _service.DepositAsync(1, 500);

        
        Assert.Equal(1500, account.Balance);
    }

    [Fact]
    public async Task WithdrawAsync_ShouldFail_IfInsufficientFunds()
    {
        
        var account = new Account { Id = 1, Balance = 500 };
        _mockRepo.Setup(r => r.GetAccountByIdAsync(1)).ReturnsAsync(account);

        
        await Assert.ThrowsAsync<System.Exception>(() => _service.WithdrawAsync(1, 1000));
    }
}