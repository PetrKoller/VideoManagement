namespace VideoManagement.Contracts.Api.V1;

public static class VideoClientErrors
{
    public static Failure VideoUploadStartFailed() => new("VideoClient.UploadStart", "Failed to start video upload");

    public static Failure SignedResourceFailed() => new("VideoClient.SignedResource", "Failed to get signed resource");

    public static Failure DownloadUrlFailed() => new("VideoClient.DownloadUrl", "Failed to get download url");
}
