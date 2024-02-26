var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddTokenService()
    .AddTransient<AuthMessageHandler>()
    .AddTransient<TestService>()
    .AddHttpClient<TestClient>((sp, c) =>
    {
        var settings = sp.GetRequiredService<IOptions<TestSettings>>().Value;
        c.BaseAddress = new Uri(settings.TestServer);
    })
    .AddHttpMessageHandler<AuthMessageHandler>();

var app = builder.Build();

try
{
    app.Run();
}
catch (Exception e)
{
    Console.WriteLine(e);
}
