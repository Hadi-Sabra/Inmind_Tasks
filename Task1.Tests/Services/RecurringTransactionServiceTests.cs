public class RecurringTransactionServiceTests
{
    private readonly Mock<IRecurringTransactionRepository> _mockRepo;
    private readonly RecurringTransactionService _service;

    public RecurringTransactionServiceTests()
    {
        _mockRepo = new Mock<IRecurringTransactionRepository>();
        _service = new RecurringTransactionService(_mockRepo.Object);
    }

    [Fact]
    public async Task ProcessRecurringTransaction_ShouldUpdateAccountBalance()
    {
        var account = new Account { Id = Guid.NewGuid(), Balance = 1000 };
        var transaction = new RecurringTransaction { AccountId = account.Id, Amount = 100, Frequency = "Monthly" };

        _mockRepo.Setup(r => r.GetRecurringTransactions()).ReturnsAsync(new List<RecurringTransaction> { transaction });
        _mockRepo.Setup(r => r.ProcessTransaction(It.IsAny<RecurringTransaction>())).ReturnsAsync(true);

        await _service.ProcessRecurringTransactions();

        Assert.Equal(900, account.Balance);
    }
}