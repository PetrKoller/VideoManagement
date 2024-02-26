namespace VideoManagement.Features.Videos.Encode.EncodingFailed;

public sealed record FailEncodingCommand(string JobId, string ErrorMessage) : IRequest<OperationResult<Success>>;

public sealed class FailEncodingCommandHandler : IRequestHandler<FailEncodingCommand, OperationResult<Success>>
{
    private readonly IVideoRepository _videoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FailEncodingCommandHandler> _logger;

    public FailEncodingCommandHandler(
        IVideoRepository videoRepository,
        IUnitOfWork unitOfWork,
        ILogger<FailEncodingCommandHandler> logger)
    {
        _videoRepository = videoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async ValueTask<OperationResult<Success>> Handle(FailEncodingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing failed encoding job. JobId: {JobId}", request.JobId);
        var video = await _videoRepository.GetByJobIdAsync(request.JobId, cancellationToken);

        if (video is null)
        {
            _logger.LogWarning("Cannot find video with given job id. JobId: {JobId}", request.JobId);
            return VideoErrors.NotFound;
        }

        video.EncodingFailed(request.ErrorMessage);
        _ = await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Processing failed encoding job finished. JobId: {JobId}", request.JobId);
        return default(Success);
    }
}
