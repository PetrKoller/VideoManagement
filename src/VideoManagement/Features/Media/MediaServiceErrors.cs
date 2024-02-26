namespace VideoManagement.Features.Media;

public static class MediaServiceErrors
{
    public static Failure JobCreationFailed()
        => new("MediaService.JobCreationFailed", "Error while creating encoding job.");
}
