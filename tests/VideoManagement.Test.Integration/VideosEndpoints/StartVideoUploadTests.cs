namespace VideoManagement.Test.Integration.VideosEndpoints;

[Collection("Shared test collection")]
public class StartVideoUploadTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly HttpClient _unauthorizedClient;
    private readonly Func<Task> _resetDatabase;
    private readonly ApiFactory _factory;

    public StartVideoUploadTests(ApiFactory apiFactory)
    {
        _factory = apiFactory;
        _resetDatabase = apiFactory.ResetDatabase;
        _client = apiFactory.HttpClient;
        _unauthorizedClient = apiFactory.UnauthorizedClient;
    }

    [Fact]
    public async Task StartVideoUpload_ReturnsUnauthorized_WhenUserNotAuthenticated()
    {
        // Arrange
        var command = new StartVideoUpload("Valid Video Name", "Valid Owner Name");

        // Act
        var response = await _unauthorizedClient.PostAsJsonAsync("/api/video-management/videos/start-upload", command);

        // Assert
        _ = response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task StartVideoUpload_ReturnsForbidden_WhenNotSufficientPermissions()
    {
        // Arrange
        var command = new StartVideoUpload("Valid Video Name", "Valid Owner Name");

        // Act
        var request = TestHelpers.CreatePostRequest(
            "/api/video-management/videos/start-upload",
            command,
            [Permissions.PlayVideo]);
        var response = await _client.SendAsync(request);

        // Assert
        _ = response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task StartVideoUpload_ReturnsOkResponse_WhenValidData()
    {
        // Arrange
        var command = new StartVideoUpload("Valid Video Name", "Valid Owner Name");
        var expectedResponse = new VideoUploadResponse(Guid.NewGuid(), "https://test.com");
        var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Act
        var request = TestHelpers.CreatePostRequest(
            "/api/video-management/videos/start-upload",
            command,
            [Permissions.UploadVideo]);
        var response = await _client.SendAsync(request);

        // Assert
        var returnedResponse = await response.Content.ReadFromJsonAsync<VideoUploadResponse>();
        _ = response.StatusCode.Should().Be(HttpStatusCode.OK);
        _ = returnedResponse.Should().NotBeNull();
        _ = returnedResponse?.UploadUrl.Should().Be(expectedResponse.UploadUrl);

        var video = await dbContext.Videos.FirstOrDefaultAsync(x => x.ExternalId == returnedResponse!.VideoId);
        _ = video.Should().NotBeNull();
        _ = video?.Status.Should().Be(VideoStatus.Uploading);
    }

    [Fact]
    public async Task StartVideoUpload_ReturnsBadRequest_WhenInvalidData()
    {
        // Arrange
        var command = new StartVideoUpload(string.Empty, "Valid Owner Name");

        // Act
        var request = TestHelpers.CreatePostRequest(
            "/api/video-management/videos/start-upload",
            command,
            [Permissions.UploadVideo]);
        var response = await _client.SendAsync(request);

        // Assert
        _ = response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}
