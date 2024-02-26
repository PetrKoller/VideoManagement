namespace VideoManagement.Features.Videos.Encode.EncodingCompleted;

public record CompleteEncodingCommand(MediaConvertCompletedEvent Event) : IRequest<OperationResult<Success>>;

public sealed class ProcessCompletedEncodingCommandHandler : IRequestHandler<CompleteEncodingCommand, OperationResult<Success>>
{
    private readonly IVideoRepository _videoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProcessCompletedEncodingCommandHandler> _logger;

    public ProcessCompletedEncodingCommandHandler(
        IVideoRepository videoRepository,
        IUnitOfWork unitOfWork,
        ILogger<ProcessCompletedEncodingCommandHandler> logger)
    {
        _videoRepository = videoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async ValueTask<OperationResult<Success>> Handle(CompleteEncodingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing completed encoding job. JobId: {JobId}", request.Event.Detail.JobId);
        var video = await _videoRepository.GetByJobIdAsync(request.Event.Detail.JobId, cancellationToken);

        if (video is null)
        {
            _logger.LogWarning("Cannot find video with given job id. JobId: {JobId}", request.Event.Detail.JobId);
            return VideoErrors.NotFound;
        }

        string streamPath = ExtractRelativeS3FileUri(request.Event.Detail
            .OutputGroupDetails.Single(x => x.Type == MediaConvertCompletedEvent.HlsGroupName)
            .OutputDetails.First()
            .OutputFilePaths.First());

        string? downloadPath = null;

        if (video.IsDownloadable)
        {
            downloadPath = ExtractRelativeS3FileUri(request.Event.Detail
                .OutputGroupDetails.Single(x => x.Type == MediaConvertCompletedEvent.FileGroupName)
                .OutputDetails.First()
                .OutputFilePaths.First());
        }

        video.EncodingCompleted(DateTime.UtcNow, streamPath, downloadPath);
        _ = await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Processing completed encoding job finished. JobId: {JobId}", request.Event.Detail.JobId);
        return default(Success);
    }

    private static string ExtractRelativeS3FileUri(string path)
        => path[(path.IndexOf('/', path.IndexOf("//", StringComparison.Ordinal) + 2) + 1)..];
}
