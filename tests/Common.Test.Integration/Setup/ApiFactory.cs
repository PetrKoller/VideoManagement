namespace Common.Test.Integration.Setup;

public sealed class ApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    public ApiFactory()
    {
        TestHttpServer = new TestHttpServer();
        TestHttpServer.Start();
    }

    public TestHttpServer TestHttpServer { get; init; }

    public Task InitializeAsync() => Task.CompletedTask;

    Task IAsyncLifetime.DisposeAsync()
    {
        TestHttpServer.Dispose();
        return Task.CompletedTask;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _ = builder.ConfigureLogging(c => c.ClearProviders());
        _ = builder.ConfigureTestServices(services =>
            services
                .Configure<JwtSettings>(x =>
                {
                    x.AuthServer = TestHttpServer.Url;
                    x.ClientId = TestConstants.ValidClientId;
                    x.ClientSecret = "test-secret";
                    x.Realm = "PowerTrainer";
                })
                .Configure<TestSettings>(x => x.TestServer = TestHttpServer.Url));
    }
}
