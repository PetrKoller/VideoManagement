namespace VideoManagement.Database;

public class AppDbContext : DbContext, IUnitOfWork
{
    private readonly DatabaseOptions _options;
    private readonly IWebHostEnvironment _environment;

    public AppDbContext(
        IOptions<DatabaseOptions> options,
        IWebHostEnvironment environment)
    {
        _environment = environment;
        _options = options.Value;
    }

    public DbSet<Video> Videos { get; set; } = default!;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await AddDomainEventsAsOutboxMessages(cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        _ = optionsBuilder
            .UseNpgsql(_options.ConnectionString)
            .UseSnakeCaseNamingConvention();

        if (_environment.IsDevelopment())
        {
            _ = optionsBuilder
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddSerilog()));
        }

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.ApplyConfiguration(new VideoEntityConfiguration());
        _ = modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());

        base.OnModelCreating(modelBuilder);
    }

    private Task AddDomainEventsAsOutboxMessages(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(x => x.Entity)
            .SelectMany(entity =>
            {
                var events = entity.GetDomainEvents();
                entity.ClearDomainEvents();
                return events;
            })
            .Select(x =>
            {
                var type = x.GetType();
                return new OutboxMessage(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    type.FullName ?? throw new InvalidOperationException("Cannot get type name"),
                    JsonSerializer.Serialize(x, type));
            })
            .ToList();

        return AddRangeAsync(domainEvents, cancellationToken);
    }
}
