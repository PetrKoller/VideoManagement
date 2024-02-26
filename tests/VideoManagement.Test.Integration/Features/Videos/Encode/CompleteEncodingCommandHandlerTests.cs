namespace VideoManagement.Test.Integration.Features.Videos.Encode;

[Collection("Shared test collection")]
public class CompleteEncodingCommandHandlerTests : IAsyncLifetime
{
    private readonly Func<Task> _resetDatabase;
    private readonly ISender _sender;
    private readonly AppDbContext _dbContext;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public CompleteEncodingCommandHandlerTests(ApiFactory apiFactory)
    {
        var scope = apiFactory.Services.CreateScope();
        _sender = scope.ServiceProvider.GetRequiredService<ISender>();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();
        _resetDatabase = apiFactory.ResetDatabase;
    }

    [Fact]
    public async Task CompleteEncodingCommandHandler_ReturnsNotFound_WhenVideoNotFound()
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

        var command =
            new CompleteEncodingCommand(CreateMediaConvertCompletedEvent("nonExistingJobId"));

        // Act
        var result = await _sender.Send(command);

        // Assert
        _ = result.Failure.Should().Be(VideoErrors.NotFound);
    }

    [Fact]
    public async Task CompleteEncodingCommandHandler_ReturnsFailure_WhenInvalidData()
    {
        // Arrange
        var command =
            new CompleteEncodingCommand(new MediaConvertCompletedEvent(
                new CompletedDetail(
                    "random job id",
                    "COMPLETE",
                    [])));

        // Act
        var result = await _sender.Send(command);

        // Assert
        _ = result.IsFailure.Should().BeTrue();
        _ = result.Failure.Should().NotBe(VideoErrors.NotFound);
    }

    [Fact]
    public async Task CompleteEncodingCommandHandler_ReturnsSuccess_WhenCorrespondingVideoExists()
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

        var command =
            new CompleteEncodingCommand(CreateMediaConvertCompletedEvent("testJobId"));

        // Act
        var result = await _sender.Send(command);
        var modifiedVideo = await _dbContext.Videos.FirstOrDefaultAsync(x => x.Id == video.Id);
        var outboxMessages = await _sqlConnectionFactory.GetAllOutboxMessages();

        // Assert
        _ = result.IsFailure.Should().BeFalse();
        _ = result.Data.Should().Be(default(Success));
        _ = modifiedVideo.Should().NotBeNull();
        _ = modifiedVideo!.ExternalId.Should().Be(video.ExternalId);
        _ = modifiedVideo.Status.Should().Be(VideoStatus.Completed);
        _ = modifiedVideo.StreamFileLocation.Should().Be("testStreamingPath");
        _ = modifiedVideo.DownloadFileLocation.Should().Be("testDownloadPath");
        _ = outboxMessages
            .Should().HaveCount(1)
            .And.Contain(x => x.Type == typeof(EncodingCompletedDomainEvent).FullName);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    private static MediaConvertCompletedEvent CreateMediaConvertCompletedEvent(string jobId) => new(
        new CompletedDetail(
            jobId,
            "COMPLETE",
            [
                new(
                    [new(["testStreamingPath"])],
                    MediaConvertCompletedEvent.HlsGroupName),

                new(
                    [new(["testDownloadPath"])],
                    MediaConvertCompletedEvent.FileGroupName)

            ]));
}
