namespace Common.Test.Unit.TokenServicesTests;

public class TokenServiceTests
{
    private readonly IMemoryCache _cache = Substitute.For<IMemoryCache>();
    private readonly ITokenClient _tokenClient = Substitute.For<ITokenClient>();
    private readonly ILogger<TokenService> _logger = Substitute.For<ILogger<TokenService>>();
    private readonly TokenService _sut;

    public TokenServiceTests() =>
        _sut = new TokenService(_cache, _tokenClient, _logger);

    [Fact]
    public async Task GetTokenAsync_ReturnsToken_WhenInCache()
    {
        // Arrange
        const string expectedValue = "returned-access-token";
        _ = _cache.TryGetValue("access-token", out string? _).Returns(x =>
        {
            x[1] = expectedValue;
            return true;
        });

        // Act
        var result = await _sut.GetTokenAsync();

        // Assert
        _ = result.IsFailure.Should().BeFalse();
        _ = result.Data.Should().BeEquivalentTo(expectedValue);
    }

    [Fact]
    public async Task GetTokenAsync_ReturnsToken_WhenReturnedFromClient()
    {
        // Arrange
        _ = _cache.TryGetValue("access-token", out string? _).Returns(x =>
        {
            x[1] = null;
            return false;
        });
        var tokenResponse = new TokenResponse(
            "test-access-token",
            "test-refresh-token",
            "test-id-token",
            50,
            50);
        _ = _tokenClient.RequestAccessTokenAsync().Returns(tokenResponse);

        var result = await _sut.GetTokenAsync();

        _ = result.IsFailure.Should().BeFalse();
        _ = result.Data.Should().BeEquivalentTo(tokenResponse.AccessToken);
    }

    [Fact]
    public async Task GetTokenAsync_ReturnsFailure_WhenTokenClientFails()
    {
        // Arrange
        _ = _cache.TryGetValue("access-token", out string? _).Returns(x =>
        {
            x[1] = null;
            return false;
        });

        _ = _tokenClient.RequestAccessTokenAsync().Returns(TokenClientErrors.FailedToGetToken());

        var result = await _sut.GetTokenAsync();

        _ = result.IsFailure.Should().BeTrue();
        _ = result.Failure.Should().BeEquivalentTo(TokenClientErrors.FailedToGetToken());
    }
}
