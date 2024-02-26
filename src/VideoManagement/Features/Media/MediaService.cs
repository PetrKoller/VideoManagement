namespace VideoManagement.Features.Media;

public sealed class MediaService : IMediaService
{
    private const string VideoIdTag = "VideoId";

    private readonly IAmazonMediaConvert _mediaConvert;
    private readonly MediaServiceOptions _mediaServiceOptions;
    private readonly BlobStorageOptions _storageOptions;
    private readonly ILogger<MediaService> _logger;

    public MediaService(
        IAmazonMediaConvert mediaConvert,
        IOptions<MediaServiceOptions> mediaServiceOptions,
        IOptions<BlobStorageOptions> storageOptions,
        ILogger<MediaService> logger)
    {
        _mediaConvert = mediaConvert;
        _logger = logger;
        _storageOptions = storageOptions.Value;
        _mediaServiceOptions = mediaServiceOptions.Value;
    }

    public async Task<OperationResult<string>> CreateEncodingJobAsync(
        Guid videoId,
        string originalFileLocation,
        string destinationFileLocation,
        bool isDownloadable)
    {
        string s3Input = $"s3://{_storageOptions.BucketName}/{originalFileLocation}";
        string s3Destination = $"s3://{_storageOptions.BucketName}/{destinationFileLocation}/";

        var builder = new CreateJobRequestBuilder()
            .WithRole(_mediaServiceOptions.MediaConvertRole)
            .WithInput(s3Input)
            .WithHlsOutputGroup(s3Destination)
            .WithTag(VideoIdTag, videoId.ToString());

        if (isDownloadable)
        {
            _ = builder.WithFileOutput(s3Destination);
        }

        var createJobRequest = builder.Build();

        try
        {
            var res = await _mediaConvert.CreateJobAsync(createJobRequest);
            return res.Job.Id;
        }
        catch (AmazonServiceException e)
        {
            _logger.LogError(e, "Error while creating encoding job. VideoId: {VideoId}", videoId);
            return MediaServiceErrors.JobCreationFailed();
        }
    }
}
