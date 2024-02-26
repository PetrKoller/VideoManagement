namespace Common.Auth.TokenServices;

public static class TokenServiceExtensions
{
    /// <summary>
    /// Add token service to DI container.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>Service collection with registered <see cref="ITokenService"/> and its dependencies.</returns>
    public static IServiceCollection AddTokenService(this IServiceCollection services)
    {
        _ = services
            .AddMemoryCache()
            .AddSingleton<ITokenService, TokenService>()
        /*
         Usage of typed http client in singleton (long lived client with custom socket https handler pool, so http client factory handler can be turned off)
         https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines#recommended-use
         https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory#avoid-typed-clients-in-singleton-services
         https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory#using-ihttpclientfactory-together-with-socketshttphandler
         */
            .AddHttpClient<ITokenClient, KeycloakCredentialsClient>((sp, c) =>
            {
                var jwtOptions = sp.GetRequiredService<IOptions<JwtSettings>>().Value;
                string keycloakUrl = jwtOptions.AuthServer.EndsWith('/')
                    ? jwtOptions.AuthServer
                    : jwtOptions.AuthServer + "/";
                c.BaseAddress = new Uri(keycloakUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(5),
            })
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);

        return services;
    }
}
