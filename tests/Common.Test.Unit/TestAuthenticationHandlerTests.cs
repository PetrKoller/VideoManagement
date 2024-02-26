namespace Common.Test.Unit;

public class TestAuthenticationHandlerTests
{
    private readonly IOptionsMonitor<AuthenticationSchemeOptions> _optionsMonitor;
    private readonly ILoggerFactory _loggerFactory;
    private readonly UrlEncoder _urlEncoder;
    private readonly HttpContext _httpContext;

    public TestAuthenticationHandlerTests()
    {
        _optionsMonitor = Substitute.For<IOptionsMonitor<AuthenticationSchemeOptions>>();
        _ = _optionsMonitor.CurrentValue.Returns(new AuthenticationSchemeOptions());
        _ = _optionsMonitor.Get(Arg.Any<string>())
            .Returns(new AuthenticationSchemeOptions());
        _loggerFactory = Substitute.For<ILoggerFactory>();
        _urlEncoder = Substitute.For<UrlEncoder>();
        _httpContext = new DefaultHttpContext();
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ReturnsValidPermissionClaims_WhenPresent()
    {
        // Arrange
        string[] permissions = ["query-test", "manage-test"];
        var expectedClaims = permissions.Select(x => new Claim(AuthConstants.PermissionsClaimType, x)).ToList();

        _httpContext.Request.Headers.Authorization = $"{TestAuthConstants.AuthenticationScheme} {GetBase64HeaderValue(new TestHeaderValue(null, permissions))}";

        var handler = new TestAuthenticationHandler(_optionsMonitor, _loggerFactory, _urlEncoder);
        await handler.InitializeAsync(
            new AuthenticationScheme(
                TestAuthConstants.AuthenticationScheme,
                null,
                typeof(TestAuthenticationHandler)),
            _httpContext);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        _ = result.Succeeded.Should().BeTrue();
        _ = result.Principal.Should().NotBeNull();
        var claims = result.Principal!.Claims.Select(x => new Claim(x.Type, x.Value)).ToList();
        _ = claims.Should().BeEquivalentTo(expectedClaims);
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ReturnsValidUserIdClaim_WhenPresent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedClaims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) };

        _httpContext.Request.Headers.Authorization = $"{TestAuthConstants.AuthenticationScheme} {GetBase64HeaderValue(new TestHeaderValue(userId, []))}";
        var handler = new TestAuthenticationHandler(_optionsMonitor, _loggerFactory, _urlEncoder);
        await handler.InitializeAsync(
            new AuthenticationScheme(
                TestAuthConstants.AuthenticationScheme,
                null,
                typeof(TestAuthenticationHandler)),
            _httpContext);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        _ = result.Succeeded.Should().BeTrue();
        _ = result.Principal.Should().NotBeNull();
        var claims = result.Principal!.Claims.Select(x => new Claim(x.Type, x.Value)).ToList();
        _ = claims.Should().BeEquivalentTo(expectedClaims);
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ReturnsValidUserIdAndPermissionClaims_WhenPresent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        string[] permissions = ["query-test", "manage-test"];
        var expectedClaims = permissions.Select(x => new Claim(AuthConstants.PermissionsClaimType, x)).ToList();
        expectedClaims.Add(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));

        _httpContext.Request.Headers.Authorization = $"{TestAuthConstants.AuthenticationScheme} {GetBase64HeaderValue(new TestHeaderValue(userId, permissions))}";
        var handler = new TestAuthenticationHandler(_optionsMonitor, _loggerFactory, _urlEncoder);
        await handler.InitializeAsync(
            new AuthenticationScheme(
                TestAuthConstants.AuthenticationScheme,
                null,
                typeof(TestAuthenticationHandler)),
            _httpContext);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        _ = result.Succeeded.Should().BeTrue();
        _ = result.Principal.Should().NotBeNull();
        var claims = result.Principal!.Claims.Select(x => new Claim(x.Type, x.Value)).ToList();
        _ = claims.Should().BeEquivalentTo(expectedClaims);
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ReturnsFailAuthResult_WhenInvalidTestHeaderValue()
    {
        // Arrange
        _httpContext.Request.Headers.Authorization = $"{TestAuthConstants.AuthenticationScheme} invalid";

        var handler = new TestAuthenticationHandler(_optionsMonitor, _loggerFactory, _urlEncoder);
        await handler.InitializeAsync(
            new AuthenticationScheme(
                TestAuthConstants.AuthenticationScheme,
                null,
                typeof(TestAuthenticationHandler)),
            _httpContext);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        _ = result.Succeeded.Should().BeFalse();
        _ = result.Failure.Should().NotBeNull();
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ReturnsFailAuthResult_WhenNoSchema()
    {
        // Arrange
        _ = _optionsMonitor.CurrentValue.Returns(new AuthenticationSchemeOptions());
        _ = _optionsMonitor.Get(Arg.Any<string>())
            .Returns(new AuthenticationSchemeOptions());

        var handler = new TestAuthenticationHandler(_optionsMonitor, _loggerFactory, _urlEncoder);
        await handler.InitializeAsync(
            new AuthenticationScheme(
                TestAuthConstants.AuthenticationScheme,
                null,
                typeof(TestAuthenticationHandler)),
            _httpContext);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        _ = result.Succeeded.Should().BeFalse();
    }

    private static string GetBase64HeaderValue(TestHeaderValue testHeaderValue)
    {
        string testHeaderValueJson = JsonSerializer.Serialize(testHeaderValue);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(testHeaderValueJson));
    }
}
