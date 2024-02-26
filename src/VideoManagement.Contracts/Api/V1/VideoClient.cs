namespace VideoManagement.Contracts.Api.V1;

public class VideoClient : IVideoClient
{
    private const string VideosPath = "api/video-management/videos";

    private readonly HttpClient _httpClient;

    private readonly ILogger<VideoClient> _logger;

    public VideoClient(HttpClient httpClient, ILogger<VideoClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<OperationResult<VideoUploadResponse>> StartUpload(StartVideoUpload upload)
    {
        _logger.LogDebug("Starting upload for video {VideoName}", upload.VideoName);
        var response = await _httpClient.PostAsJsonAsync($"{VideosPath}/start-upload", upload);

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<VideoUploadResponse>())!;
        }

        _logger.LogError("Failed to start upload for video {VideoName}, {Response}", upload.VideoName, response.ReasonPhrase);
        return VideoClientErrors.VideoUploadStartFailed();
    }

    public async Task<OperationResult<SignedResource>> GetSignedResource(Guid videoId)
    {
        _logger.LogDebug("Getting download url for video with id {VideoId}", videoId);
        var response = await _httpClient.GetAsync($"{VideosPath}/{videoId}/signed-resource");

        if (response.IsSuccessStatusCode)
        {
            return (await response.Content.ReadFromJsonAsync<SignedResource>())!;
        }

        _logger.LogError("Failed to get signed resource for video with id {VideoId}, {Response}", videoId, response.ReasonPhrase);
        return VideoClientErrors.SignedResourceFailed();
    }

    public async Task<OperationResult<string>> GetDownloadUrl(Guid videoId)
    {
        _logger.LogDebug("Getting download url for video with id {VideoId}", videoId);
        var response = await _httpClient.GetAsync($"{VideosPath}/{videoId}/download-url");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }

        _logger.LogError("Failed to get download url for video with id {VideoId}, {Response}", videoId, response.ReasonPhrase);
        return VideoClientErrors.DownloadUrlFailed();
    }
}
