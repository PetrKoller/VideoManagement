namespace VideoManagement.Features.Videos.Upload;

public sealed record StartVideoUploadCommand(StartVideoUpload VideoUpload) : IRequest<OperationResult<VideoUploadResponse>>;

public sealed class StartVideoUploadCommandHandler : IRequestHandler<StartVideoUploadCommand, OperationResult<VideoUploadResponse>>
{
    private readonly IVideoRepository _videoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorage _blobStorage;
    private readonly ILogger<StartVideoUploadCommandHandler> _logger;

    public StartVideoUploadCommandHandler(
        IVideoRepository videoRepository,
        IUnitOfWork unitOfWork,
        IBlobStorage blobStorage,
        ILogger<StartVideoUploadCommandHandler> logger)
    {
        _videoRepository = videoRepository;
        _unitOfWork = unitOfWork;
        _blobStorage = blobStorage;
        _logger = logger;
    }

    public async ValueTask<OperationResult<VideoUploadResponse>> Handle(StartVideoUploadCommand request, CancellationToken cancellationToken)
    {
        var video = Video.StartUploading(
            Guid.NewGuid(),
            request.VideoUpload.VideoName,
            request.VideoUpload.OwnerName,
            Guid.NewGuid(), // TODO: Get from auth when implemented
            request.VideoUpload.IsDownloadable,
            DateTime.UtcNow);

        var createdVideo = await _videoRepository.AddAsync(video, cancellationToken);
        string uploadUrl = _blobStorage.GenerateUploadUrl(createdVideo.OriginalFileLocation);

        _ = await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Video upload process started. VideoId: {VideoId}, Location: {Location}", createdVideo.Id, createdVideo.OriginalFileLocation);

        return new VideoUploadResponse(createdVideo.ExternalId, uploadUrl);
    }
}
