namespace VideoManagement.Outbox;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOutboxProcessor(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddQuartz()
            .AddQuartzHostedService(options => options.WaitForJobsToComplete = true)
            .ConfigureOptions<ProcessOutboxMessagesJobSetup>()
            .Configure<OutboxOptions>(configuration.GetRequiredSection(OutboxOptions.SectionName));
}
