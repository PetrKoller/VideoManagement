namespace VideoManagement.Test.Integration.VideosEndpoints;

[Collection("Shared test collection")]
public class GetSignedResourceTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly HttpClient _unauthorizedClient;
    private readonly Func<Task> _resetDatabase;
    private readonly AppDbContext _dbContext;

    public GetSignedResourceTests(ApiFactory apiFactory)
    {
        var scope = apiFactory.Services.CreateScope();
        _resetDatabase = apiFactory.ResetDatabase;
        _client = apiFactory.HttpClient;
        _unauthorizedClient = apiFactory.UnauthorizedClient;
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    [Fact]
    public async Task GetSignedResource_ReturnsUnauthorized_WhenUserNotAuthenticated()
    {
        // Act
        var response = await _unauthorizedClient.GetAsync($"/api/video-management/videos/{Guid.NewGuid()}/signed-resource");

        // Assert
        _ = response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetSignedResource_ReturnsForbidden_WhenNotSufficientPermissions()
    {
        // Arrange
        var authHeaderValue = new TestHeaderValue(null, [Permissions.DownloadVideo, Permissions.UploadVideo]);
        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/video-management/videos/{Guid.NewGuid()}/signed-resource");
        request.Headers.Authorization = new AuthenticationHeaderValue(TestAuthConstants.AuthenticationScheme, TestHelpers.EncodeTestHeaderValue(authHeaderValue));

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        _ = response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetSignedResource_ReturnsSignedResource_WhenVideoDownloadable()
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
        var request = TestHelpers.CreateGetRequest($"/api/video-management/videos/{video.ExternalId}/signed-resource", [Permissions.PlayVideo]);
        var response = await _client.SendAsync(request);

        // Assert
        var result = await response.Content.ReadFromJsonAsync<SignedResource>();
        _ = response.StatusCode.Should().Be(HttpStatusCode.OK);
        _ = result.Should().NotBeNull();
        _ = result!.Url.Should().Contain(video.StreamFileLocation);
        _ = result.Policy.Should().NotBeNullOrWhiteSpace();
        _ = result.KeyPairId.Should().NotBeNullOrWhiteSpace();
        _ = result.Signature.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetSignedResource_ReturnsNotFound_WhenVideoDoesNotExist()
    {
        // Act
        var request = TestHelpers.CreateGetRequest($"/api/video-management/videos/{Guid.NewGuid()}/signed-resource", [Permissions.PlayVideo]);
        var response = await _client.SendAsync(request);

        // Assert
        _ = response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();
}
