namespace VideoManagement.Test.Integration.Features.Videos.Encode;

[Collection("Shared test collection")]
public class StartVideoEncodingCommandHandlerTests : IAsyncLifetime
{
    private readonly Func<Task> _resetDatabase;
    private readonly ISender _sender;
    private readonly AppDbContext _dbContext;

    public StartVideoEncodingCommandHandlerTests(ApiFactory apiFactory)
    {
        var scope = apiFactory.Services.CreateScope();
        _sender = scope.ServiceProvider.GetRequiredService<ISender>();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _resetDatabase = apiFactory.ResetDatabase;
    }

    [Fact]
    public async Task StartVideoEncodingCommandHandler_ReturnsFailure_WhenVideoNotFound()
    {
        // Arrange
        var command = new StartVideoEncodingCommand(Guid.NewGuid());

        // Act
        var result = await _sender.Send(command);

        // Assert
        _ = result.Failure.Should().Be(VideoErrors.NotFound);
    }

    [Fact]
    public async Task StartVideoEncodingCommandHandler_ReturnsSuccess_WhenVideoNotInUploadingState()
    {
        // Arrange
        var video = Video.StartUploading(
            Guid.NewGuid(),
            "video.mp4",
            "owner",
            Guid.NewGuid(),
            true,
            DateTime.UtcNow);

        _ = video.StartEncoding(DateTime.UtcNow);

        _ = _dbContext.Videos.Add(video);
        _ = await _dbContext.SaveChangesAsync();

        var command = new StartVideoEncodingCommand(video.ExternalId);

        // Act
        var result = await _sender.Send(command);

        // Assert
        _ = result.Data.Should().Be(default(Success));
    }

    [Fact]
    public async Task StartVideoEncodingCommandHandler_ReturnsFailure_WhenStartingEncodingJobFailed()
    {
        // Arrange
        var video = Video.StartUploading(
            Guid.Empty,
            "video.mp4",
            "owner",
            Guid.NewGuid(),
            true,
            DateTime.UtcNow);
        _ = _dbContext.Videos.Add(video);
        _ = await _dbContext.SaveChangesAsync();
        var command = new StartVideoEncodingCommand(video.ExternalId);

        // Act
        var result = await _sender.Send(command);

        // Assert
        _ = result.Failure.Should().Be(MediaServiceErrors.JobCreationFailed());
    }

    [Fact]
    public async Task StartVideoEncodingCommandHandler_ReturnsSuccess_WhenVideoEncodingJobStarted()
    {
        // Arrange
        var video = Video.StartUploading(
            Guid.NewGuid(),
            "video.mp4",
            "owner",
            Guid.NewGuid(),
            true,
            DateTime.UtcNow);
        _ = _dbContext.Videos.Add(video);
        _ = await _dbContext.SaveChangesAsync();
        var command = new StartVideoEncodingCommand(video.ExternalId);

        // Act
        var result = await _sender.Send(command);

        // Assert
        _ = result.Data.Should().Be(default(Success));
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}
