namespace VideoManagement.Features.Videos.Entity;

public sealed class Video : Common.Abstractions.Entity
{
    private Video(
        Guid externalId,
        string name,
        string ownerName,
        Guid ownerId,
        bool isDownloadable,
        VideoStatus status,
        DateTime createdOnUtc,
        string originalFileLocation,
        string destinationLocation)
    : base(externalId)
    {
        Name = name;
        OwnerName = ownerName;
        OwnerId = ownerId;
        IsDownloadable = isDownloadable;
        Status = status;
        CreatedOnUtc = createdOnUtc;
        OriginalFileLocation = originalFileLocation;
        DestinationLocation = destinationLocation;
    }

    // NOTE: This constructor is used by EF Core
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    // ReSharper disable once UnusedMember.Local
    private Video()
    {
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public string Name { get; private set; }

    public VideoStatus Status { get; private set; }

    public Guid OwnerId { get; private set; }

    public string OwnerName { get; private set; }

    public bool IsDownloadable { get; private set; }

    public string OriginalFileLocation { get; private set; }

    public string DestinationLocation { get; private set; }

    public string? StreamFileLocation { get; private set; }

    public string? DownloadFileLocation { get; private set; }

    public string? EncodingJobId { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public DateTime? UploadedOnUtc { get; private set; }

    public DateTime? EncodedOnUtc { get; private set; }

    public string? ErrorMessage { get; private set; }

    public static Video StartUploading(
        Guid externalId,
        string name,
        string ownerName,
        Guid ownerId,
        bool isDownloadable,
        DateTime utcNow)
    {
        string destination = $"{ownerName}/{utcNow:yyyyMMdd}/{externalId}";
        string originalFileLocation = $"{destination}/video-upload";

        return new Video(
            externalId,
            name,
            ownerName,
            ownerId,
            isDownloadable,
            VideoStatus.Uploading,
            utcNow,
            originalFileLocation,
            destination);
    }

    public OperationResult<Success> StartEncoding(DateTime utcNow)
    {
        if (Status != VideoStatus.Uploading)
        {
            return VideoErrors.NotUploadingState;
        }

        Status = VideoStatus.Encoding;
        UploadedOnUtc = utcNow;

        return default(Success);
    }

    public void EncodingStarted(string encodingJobId)
        => EncodingJobId = encodingJobId;

    public void EncodingFailed(string errorMessage)
    {
        Status = VideoStatus.Failed;
        ErrorMessage = errorMessage;

        RaiseDomainEvent(new EncodingFailedDomainEvent(OwnerName, OwnerId, ExternalId));
    }

    public void EncodingCompleted(
        DateTime utcNow,
        string streamFilePath,
        string? downloadFilePath = null)
    {
        EncodedOnUtc = utcNow;
        Status = VideoStatus.Completed;
        StreamFileLocation = streamFilePath;
        DownloadFileLocation = downloadFilePath;

        RaiseDomainEvent(new EncodingCompletedDomainEvent(OwnerName, OwnerId, ExternalId));
    }
}
