public class EdgeCaseServiceTests
{
    private readonly Mock<IAccountRepository> _mockRepo;
    private readonly AccountService _service;

    public EdgeCaseServiceTests()
    {
        _mockRepo = new Mock<IAccountRepository>();
        _service = new AccountService(_mockRepo.Object);
    }

    [Fact]
    public async Task CreateAccount_InvalidData_ShouldThrowException()
    {
        var account = new Account { Id = Guid.NewGuid(), Balance = -1 }; // Invalid balance

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAccount(account));
    }

    [Fact]
    public async Task Withdraw_TransactionExceedsBalance_ShouldThrowException()
    {
        var account = new Account { Id = Guid.NewGuid(), Balance = 100 };
        _mockRepo.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.Withdraw(account.Id, 200));
    }

    [Fact]
    public async Task Deposit_MaxAmount_ShouldBeProcessed()
    {
        var account = new Account { Id = Guid.NewGuid(), Balance = 100 };
        var maxDepositAmount = 1000000; // Maximum deposit
        _mockRepo.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);
        _mockRepo.Setup(r => r.UpdateAsync(account)).Returns(Task.CompletedTask);

        await _service.Deposit(account.Id, maxDepositAmount);

        Assert.Equal(account.Balance + maxDepositAmount, account.Balance);
    }
}