namespace VideoManagement.Database;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
        => services
            .AddDbContext<AppDbContext>()
            .AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>())
            .AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

    public static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }
}
