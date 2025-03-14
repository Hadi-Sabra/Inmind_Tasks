public class NotificationServiceTests
{
    private readonly Mock<INotificationRepository> _mockRepo;
    private readonly NotificationService _service;

    public NotificationServiceTests()
    {
        _mockRepo = new Mock<INotificationRepository>();
        _service = new NotificationService(_mockRepo.Object);
    }

    [Fact]
    public async Task CreateNotification_ShouldSucceed()
    {
        var notification = new Notification { Id = Guid.NewGuid(), Message = "Test Notification" };
        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Notification>())).ReturnsAsync(notification);

        var result = await _service.CreateNotification(notification);

        Assert.NotNull(result);
        Assert.Equal(notification.Message, result.Message);
    }
}