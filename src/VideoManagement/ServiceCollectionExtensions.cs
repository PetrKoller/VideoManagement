namespace VideoManagement;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSourceGeneratedMediator(this IServiceCollection services)
        => MediatorDependencyInjectionExtensions.AddMediator(
            services,
            o =>
            {
                o.Namespace = "PowerTrainer.VideoManagement.Mediator";
                o.ServiceLifetime = ServiceLifetime.Transient;
            });
}
