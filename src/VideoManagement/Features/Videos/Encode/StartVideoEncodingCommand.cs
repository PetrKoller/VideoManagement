namespace VideoManagement.Features.Videos.Encode;

public record StartVideoEncodingCommand(Guid VideoId) : IRequest<OperationResult<Success>>;

public sealed class StartVideoEncodingCommandHandler : IRequestHandler<StartVideoEncodingCommand, OperationResult<Success>>
{
    private readonly IVideoRepository _videoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediaService _mediaService;
    private readonly ILogger<StartVideoEncodingCommandHandler> _logger;

    public StartVideoEncodingCommandHandler(
        IVideoRepository videoRepository,
        IUnitOfWork unitOfWork,
        IMediaService mediaService,
        ILogger<StartVideoEncodingCommandHandler> logger)
    {
        _videoRepository = videoRepository;
        _unitOfWork = unitOfWork;
        _mediaService = mediaService;
        _logger = logger;
    }

    public async ValueTask<OperationResult<Success>> Handle(StartVideoEncodingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting video encoding process. VideoId: {VideoId}", request.VideoId);
        var video = await _videoRepository.GetByIdAsync(request.VideoId, cancellationToken);

        if (video is null)
        {
            return VideoErrors.NotFound;
        }

        var encodingResult = video.StartEncoding(DateTime.UtcNow);

        if (encodingResult.IsFailure)
        {
            _logger.LogWarning(
                "Cannot start encoding process for video, because it is not in uploading state. VideoId: {VideoId}, State: {VideoStatus}",
                request.VideoId,
                video.Status);
            return default(Success);
        }

        var result = await _mediaService.CreateEncodingJobAsync(
            video.ExternalId,
            video.OriginalFileLocation,
            video.DestinationLocation,
            video.IsDownloadable);

        if (result.TryPickFailure(out var failure, out string jobId))
        {
            _logger.LogError("Error while starting encoding job. Error: {Error}", failure.Message);
            return failure;
        }

        video.EncodingStarted(jobId);
        _ = await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Video encoding process successfully started. VideoId: {VideoId}, JobId: {JobId}", request.VideoId, jobId);
        return default(Success);
    }
}
