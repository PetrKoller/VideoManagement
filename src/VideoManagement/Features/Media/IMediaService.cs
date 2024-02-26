namespace VideoManagement.Features.Media;

public interface IMediaService
{
    Task<OperationResult<string>> CreateEncodingJobAsync(
        Guid videoId,
        string originalFileLocation,
        string destinationFileLocation,
        bool isDownloadable);
}
