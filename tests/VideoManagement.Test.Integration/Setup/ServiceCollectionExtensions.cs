namespace VideoManagement.Test.Integration.Setup;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTestDatabase(
        this IServiceCollection services,
        string connectionString)
        => services.Configure<DatabaseOptions>(o => o.ConnectionString = connectionString);

    public static IServiceCollection AddTestS3(this IServiceCollection services)
    {
        var s3Mock = Substitute.For<IAmazonS3>();

        _ = s3Mock
            .GetPreSignedURL(Arg.Any<GetPreSignedUrlRequest>())
            .Returns("https://test.com");

        _ = services
            .RemoveAll<IAmazonS3>()
            .AddSingleton<IAmazonS3>(_ => s3Mock);

        return services;
    }

    public static IServiceCollection AddTestMediaConvert(this IServiceCollection services)
    {
        var mediaConvertMock = Substitute.For<IAmazonMediaConvert>();

        _ = mediaConvertMock
            .CreateJobAsync(
            Arg.Is<CreateJobRequest>(
                x => x.Tags.ContainsValue(Guid.Empty.ToString())),
            Arg.Any<CancellationToken>())
            .ThrowsAsync(new AmazonMediaConvertException("Error"));

        _ = mediaConvertMock
            .CreateJobAsync(
                Arg.Is<CreateJobRequest>(
                    x => !x.Tags.ContainsValue(Guid.Empty.ToString())),
                Arg.Any<CancellationToken>())
            .Returns(new CreateJobResponse
            {
                Job = new Job
                {
                    Id = "test",
                },
            });

        _ = services
            .RemoveAll<IAmazonMediaConvert>()
            .AddSingleton(_ => mediaConvertMock);

        return services;
    }
}
