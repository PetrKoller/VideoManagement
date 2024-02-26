namespace Common.Test.Integration.HandlerTests;

public class AuthMessageHandlerTests : IClassFixture<ApiFactory>
{
    private readonly TestService _testService;
    private readonly IMemoryCache _memoryCache;
    private readonly TestHttpServer _testServer;

    public AuthMessageHandlerTests(ApiFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        _testService = scope.ServiceProvider.GetRequiredService<TestService>();
        _memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
        _testServer = factory.TestHttpServer;
    }

    [Fact]
    public async Task TestAuthHandler_AppendsTokenToRequest_WhenValidResponse()
    {
        // Arrange
        _testServer.SetupValidTokenResponse();

        // Act
        _ = _memoryCache.TryGetValue("access-token", out string? _).Should().BeFalse();
        var res = await _testService.CallTestEndpointAsync();

        // Assert
        _ = _memoryCache.TryGetValue("access-token", out string? token).Should().BeTrue();
        _ = token.Should().Be(TestConstants.TestTokenResponse.AccessToken);
        _ = res.Headers.Authorization.Should().NotBeNull();
        _ = res.Headers.Authorization?.Scheme.Should().Be("Bearer");
        _ = res.Headers.Authorization?.Parameter.Should().Be(TestConstants.TestTokenResponse.AccessToken);

        // Cleanup
        _memoryCache.Remove("access-token");
        _testServer.Reset();
    }

    [Fact]
    public void TestAuthHandler_ThrowsException_WhenInvalidResponse()
    {
        // Arrange
        _testServer.SetupInvalidTokenResponse();

        // Act
        _ = _memoryCache.TryGetValue("access-token", out string? _).Should().BeFalse();
        var act = _testService.CallTestEndpointAsync;

        // Assert
        _ = act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage(TokenClientErrors.FailedToGetToken().Message);
        _ = _memoryCache.TryGetValue("access-token", out string? _).Should().BeFalse();

        // Cleanup
        _testServer.Reset();
    }
}
