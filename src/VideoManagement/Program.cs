var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Configuration.AddEnvironmentVariables("VIDEO_MANAGEMENT_");

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services
    .AddJwtAuthentication(builder.Configuration)
    .AddAuthorizationPolicies([
        Permissions.DownloadVideo,
        Permissions.UploadVideo,
        Permissions.PlayVideo
    ])
    .AddServiceOptions(builder.Configuration)
    .AddEndpointsApiExplorer()
    .AddCors()
    .AddPowerTrainerSwagger(WebConstants.AppName)
    .AddDatabase()
    .AddFeatures(builder.Configuration)
    .AddOutboxProcessor(builder.Configuration)
    .AddSourceGeneratedMediator() // Must be registered like this because it clashed with masstransit mediator
    .AddMassTransit(x =>
    {
        x.SetKebabCaseEndpointNameFormatter();
        x.UsingRabbitMq((context, configurator) =>
        {
            configurator.Host(builder.Configuration["MessageBroker:Host"], h =>
            {
                h.Username(builder.Configuration["MessageBroker:Username"]);
                h.Password(builder.Configuration["MessageBroker:Password"]);
            });
            configurator.ConfigureEndpoints(context);
        });
    })
    .AddHealthChecks()
    .AddNpgSql(sp => sp.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString);

var app = builder.Build();

app
    .UseMiddleware<ExceptionHandlingMiddleware>()
    .UseSerilogRequestLogging()
    .UseCors(c => c
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

app.UseAuthentication()
    .UseAuthorization();

app
    .MapVideoEndpoints()
    .MapHealthChecks("/health");

if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();

    app.MigrateDatabase();
}

app.Run();
