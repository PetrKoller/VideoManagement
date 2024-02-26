namespace Common.Auth.TestHelpers;

public static class TestHelpersExtensions
{
    /// <summary>
    /// Registers test authentication. Should be used only in integration tests.
    /// Test auth works only with <see cref="T:Common.Auth.TestHelpers.TestHeaderValue" />.
    /// Where can be specified user id and permissions that are parsed.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>Service collection with added test authentication.</returns>
    public static IServiceCollection AddTestAuth(this IServiceCollection services)
    {
        _ = services.Configure<JwtSettings>(o => o.AuthenticationScheme = TestAuthConstants.AuthenticationScheme);

        _ = services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = TestAuthConstants.AuthenticationScheme;
                o.DefaultScheme = TestAuthConstants.AuthenticationScheme;
                o.DefaultChallengeScheme = TestAuthConstants.AuthenticationScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                TestAuthConstants.AuthenticationScheme,
                _ => { });

        return services;
    }
}
