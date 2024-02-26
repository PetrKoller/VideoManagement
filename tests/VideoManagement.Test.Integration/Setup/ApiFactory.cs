namespace VideoManagement.Test.Integration.Setup;

public sealed class ApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithCleanUp(true)
        .Build();

    private string _connectionString = default!;
    private DbConnection _dbConnection = default!;
    private Respawner _respawner = default!;

    public HttpClient HttpClient { get; private set; } = default!;

    public HttpClient UnauthorizedClient { get; private set; } = default!;

    public async Task ResetDatabase() => await _respawner.ResetAsync(_dbConnection);

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        var connBuilder = new NpgsqlConnectionStringBuilder(_dbContainer.GetConnectionString())
        {
            Database = "powertrainer-video-management",
        };

        _connectionString = connBuilder.ConnectionString;
        _dbConnection = new NpgsqlConnection(_connectionString);
        UnauthorizedClient = CreateClient();
        HttpClient = CreateClient();
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthConstants.AuthenticationScheme);

        await InitializeRespawner();
    }

    async Task IAsyncLifetime.DisposeAsync() => await _dbContainer.DisposeAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _ = builder.ConfigureLogging(c => c.ClearProviders());
        _ = builder.ConfigureTestServices(services
            => services
                .RemoveAll<IHostedService>()
                .AddTestDatabase(_connectionString)
                .AddTestS3()
                .AddTestMediaConvert()
                .RemoveAll<IResourceSigner>()
                .AddSingleton<IResourceSigner>(new ResourceSigner(Microsoft.Extensions.Options.Options.Create(new CloudFrontOptions
                {
                    Url = "https://test.com",
                    PublicKeyId = "test",
                    PrivateKeyLocation = "./FakeKey.xml",
                    LinkLifetimeInMinutes = 10,
                })))
                .AddTestAuth());
    }

    private async Task InitializeRespawner()
    {
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(
            _dbConnection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = ["public"],
            });
    }
}
