namespace Common.Auth.TestHelpers;

/// <summary>
/// Test authentication handler for testing purposes in integration tests.
/// </summary>
/// <param name="options">Authentication scheme options.</param>
/// <param name="logger">Logger factory.</param>
/// <param name="encoder">Url encoder.</param>
public class TestAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Context.Request.Headers.Authorization.ToString().StartsWith(TestAuthConstants.AuthenticationScheme))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        string value = Context.Request.Headers.Authorization.ToString()[TestAuthConstants.AuthenticationScheme.Length..].Trim();

        if (string.IsNullOrEmpty(value))
        {
            return Task.FromResult(AuthenticateResult.Fail("No token provided"));
        }

        TestHeaderValue testHeaderValue;
        try
        {
            string json = Encoding.UTF8.GetString(Convert.FromBase64String(value));
            testHeaderValue = JsonSerializer.Deserialize<TestHeaderValue>(json)!;
        }
        catch (Exception e)
        {
            return Task.FromResult(AuthenticateResult.Fail(e));
        }

        var claims = testHeaderValue.Permissions.Select(x => new Claim(AuthConstants.PermissionsClaimType, x)).ToList();

        if (testHeaderValue.UserId is not null)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, testHeaderValue.UserId.Value.ToString()));
        }

        var identity = new ClaimsIdentity(claims, TestAuthConstants.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var result = new AuthenticationTicket(principal, TestAuthConstants.AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(result));
    }
}
