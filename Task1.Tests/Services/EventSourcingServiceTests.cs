public class EventSourcingServiceTests
{
    private readonly Mock<IEventSourcingRepository> _mockRepo;
    private readonly EventSourcingService _service;

    public EventSourcingServiceTests()
    {
        _mockRepo = new Mock<IEventSourcingRepository>();
        _service = new EventSourcingService(_mockRepo.Object);
    }

    [Fact]
    public async Task RollbackEvent_ShouldRevertChanges()
    {
        var eventToRevert = new TransactionEvent { Id = Guid.NewGuid(), EventType = "Deposit", Amount = 100 };
        _mockRepo.Setup(r => r.GetEventById(It.IsAny<Guid>())).ReturnsAsync(eventToRevert);
        _mockRepo.Setup(r => r.RollbackEvent(It.IsAny<TransactionEvent>())).ReturnsAsync(true);

        var result = await _service.RollbackEvent(eventToRevert.Id);

        Assert.True(result);
    }
}