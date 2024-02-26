namespace VideoManagement.Test.Integration.VideosEndpoints;

[Collection("Shared test collection")]
public class GetDownloadLinkTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly HttpClient _unauthorizedClient;
    private readonly Func<Task> _resetDatabase;
    private readonly AppDbContext _dbContext;

    public GetDownloadLinkTests(ApiFactory apiFactory)
    {
        var scope = apiFactory.Services.CreateScope();
        _resetDatabase = apiFactory.ResetDatabase;
        _client = apiFactory.HttpClient;
        _unauthorizedClient = apiFactory.UnauthorizedClient;
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    [Fact]
    public async Task GetDownloadLink_ReturnsUnauthorized_WhenUserNotAuthenticated()
    {
        // Act
        var response = await _unauthorizedClient.GetAsync($"/api/video-management/videos/{Guid.NewGuid()}/download-link");

        // Assert
        _ = response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDownloadLink_ReturnsForbidden_WhenNotSufficientPermissions()
    {
        // Arrange
        var authHeaderValue = new TestHeaderValue(null, [Permissions.PlayVideo]);
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/video-management/videos/{Guid.NewGuid()}/download-link");
        request.Headers.Authorization = new AuthenticationHeaderValue(TestAuthConstants.AuthenticationScheme, TestHelpers.EncodeTestHeaderValue(authHeaderValue));

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        _ = response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetDownloadLink_ReturnsDownloadUrl_WhenVideoDownloadable()
    {
        // Arrange
        var video = Video.StartUploading(
            Guid.NewGuid(),
            "Test",
            "test",
            Guid.NewGuid(),
            true,
            DateTime.UtcNow);
        video.EncodingCompleted(DateTime.UtcNow, video.DestinationLocation + "/test", video.DestinationLocation + "/test-stream");
        _ = _dbContext.Videos.Add(video);
        _ = await _dbContext.SaveChangesAsync();

        // Act
        var request = TestHelpers.CreateGetRequest($"/api/video-management/videos/{video.ExternalId}/download-link", [Permissions.DownloadVideo]);
        var response = await _client.SendAsync(request);

        // Assert
        string link = await response.Content.ReadAsStringAsync();
        _ = link.Should().Contain(video.DownloadFileLocation);
        _ = response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDownloadLink_ReturnsNotFound_WhenVideoNotDownloadable()
    {
        // Arrange
        var video = Video.StartUploading(
            Guid.NewGuid(),
            "Test",
            "test",
            Guid.NewGuid(),
            false,
            DateTime.UtcNow);
        video.EncodingCompleted(DateTime.UtcNow, video.DestinationLocation + "/test");
        _ = _dbContext.Videos.Add(video);
        _ = await _dbContext.SaveChangesAsync();

        // Act
        var request = TestHelpers.CreateGetRequest($"/api/video-management/videos/{video.ExternalId}/download-link", [Permissions.DownloadVideo]);
        var response = await _client.SendAsync(request);

        // Assert
        _ = response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetDownloadLink_ReturnsNotFound_WhenVideoDoesNotExist()
    {
        // Act
        var request = TestHelpers.CreateGetRequest($"/api/video-management/videos/{Guid.NewGuid()}/download-link", [Permissions.DownloadVideo]);
        var response = await _client.SendAsync(request);

        // Assert
        _ = response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}
