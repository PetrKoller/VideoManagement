namespace VideoManagement.Test.Integration.Features.Videos.Encode;

[Collection("Shared test collection")]
public class FailEncodingCommandHandlerTests : IAsyncLifetime
{
    private readonly Func<Task> _resetDatabase;
    private readonly ISender _sender;
    private readonly AppDbContext _dbContext;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public FailEncodingCommandHandlerTests(ApiFactory apiFactory)
    {
        var scope = apiFactory.Services.CreateScope();
        _sender = scope.ServiceProvider.GetRequiredService<ISender>();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
        _resetDatabase = apiFactory.ResetDatabase;
    }

    [Fact]
    public async Task FailEncodingCommandHandler_ReturnsNotFound_WhenVideoNotFound()
    {
        // Arrange
        var command = new FailEncodingCommand("nonExistingJobId", "test error");

        // Act
        var result = await _sender.Send(command);

        // Assert
        _ = result.Failure.Should().Be(VideoErrors.NotFound);
    }

    [Fact]
    public async Task FailEncodingCommandHandler_ReturnsSuccess_WhenVideoExists()
    {
        // Arrange
        var video = Video.StartUploading(
            Guid.NewGuid(),
            "Test",
            "test",
            Guid.NewGuid(),
            true,
            DateTime.UtcNow);
        _ = video.StartEncoding(DateTime.UtcNow);
        video.EncodingStarted("testJobId");
        _ = _dbContext.Videos.Add(video);
        _ = await _dbContext.SaveChangesAsync();
        var command = new FailEncodingCommand("testJobId", "test error");

        // Act
        var result = await _sender.Send(command);
        var modifiedVideo = await _dbContext.Videos.FirstOrDefaultAsync(x => x.Id == video.Id);
        var outboxMessages = await _sqlConnectionFactory.GetAllOutboxMessages();

        // Assert
        _ = result.Data.Should().Be(default(Success));
        _ = modifiedVideo.Should().NotBeNull();
        _ = modifiedVideo!.ExternalId.Should().Be(video.ExternalId);
        _ = modifiedVideo.Status.Should().Be(VideoStatus.Failed);
        _ = modifiedVideo.ErrorMessage.Should().Be("test error");
        _ = outboxMessages
            .Should().HaveCount(1)
            .And.Contain(x => x.Type == typeof(EncodingFailedDomainEvent).FullName);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}
