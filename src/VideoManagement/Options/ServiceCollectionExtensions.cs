namespace VideoManagement.Options;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServiceOptions(
        this IServiceCollection services,
        IConfiguration configuration)
        => services
            .Configure<DatabaseOptions>(configuration.GetRequiredSection(DatabaseOptions.SectionName))
            .Configure<BlobStorageOptions>(configuration.GetRequiredSection(BlobStorageOptions.SectionName))
            .Configure<SqsOptions>(configuration.GetRequiredSection(SqsOptions.SectionName))
            .Configure<MediaServiceOptions>(configuration.GetRequiredSection(MediaServiceOptions.SectionName))
            .Configure<CloudFrontOptions>(configuration.GetRequiredSection(CloudFrontOptions.SectionName));
}
