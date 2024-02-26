namespace VideoManagement.Features;

public static class FeaturesExtensions
{
    public static IServiceCollection AddFeatures(
        this IServiceCollection services,
        IConfiguration configuration)
        => services
            .AddRepositories()
            .AddAwsServices(configuration)
            .AddSingleton<IBlobStorage, S3Storage>()
            .AddSingleton<IMediaService, MediaService>()
            .AddSingleton<IResourceSigner, ResourceSigner>()
            .AddBackgroundServices()
            .AddValidations();

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    => services.AddScoped<IVideoRepository, VideoRepository>();

    private static IServiceCollection AddAwsServices(
        this IServiceCollection services,
        IConfiguration configuration)
        => services
            .AddDefaultAWSOptions(configuration.GetAWSOptions())
            .AddAWSService<IAmazonS3>()
            .AddAWSService<IAmazonSQS>()
            .AddAWSService<IAmazonMediaConvert>(new AWSOptions
            {
                DefaultClientConfig = { ServiceURL = configuration["MediaService:MediaConvertEndpoint"] },
            });

    private static IServiceCollection AddBackgroundServices(this IServiceCollection services)
        => services
            .AddHostedService<VideoUploadedConsumer>()
            .AddHostedService<EncodingCompletedConsumer>()
            .AddHostedService<EncodingFailedConsumer>();

    private static IServiceCollection AddValidations(this IServiceCollection services)
        => services
            .AddValidatorsFromAssemblyContaining<StartVideoUploadCommand>()
            .AddValidationBehavior<StartVideoUploadCommand, VideoUploadResponse>()
            .AddValidationBehavior<CompleteEncodingCommand, Success>();
}
