namespace VideoManagement.Contracts.Api.V1;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Add video management api with its dependencies to DI container.
    /// </summary>
    /// <param name="services"> Service collection. </param>
    /// <param name="baseUrlFactory"> Base url factory. </param>
    /// <returns> IHttpClientBuilder with registered <see cref="IVideoClient" /> and its dependencies. </returns>
    public static IHttpClientBuilder AddVideoManagementApi(
        this IServiceCollection services,
        Func<IServiceProvider, string> baseUrlFactory)
    {
        // Registered here because it's used by the AuthMessageHandler
        _ = services
            .AddTokenService()
            .AddTransient<AuthMessageHandler>();
        return services.AddHttpClient<IVideoClient, VideoClient>((sp, c) =>
        {
            string videoManagementApiUrl = baseUrlFactory(sp);
            videoManagementApiUrl = videoManagementApiUrl.EndsWith('/')
                ? videoManagementApiUrl
                : videoManagementApiUrl + "/";
            c.BaseAddress = new Uri(videoManagementApiUrl);
        }).AddHttpMessageHandler<AuthMessageHandler>();
    }
}
