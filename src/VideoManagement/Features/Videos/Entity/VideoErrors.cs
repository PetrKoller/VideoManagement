namespace VideoManagement.Features.Videos.Entity;

public static class VideoErrors
{
    public static Failure NotUploadingState
        => new("Video.NotUploadState", "Video is not in uploading state and cannot be further processed");

    public static Failure NotFound
        => new("Video.NotFound", "Video not found");
}
