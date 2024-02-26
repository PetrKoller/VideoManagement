namespace VideoManagement.Options;

public class MediaServiceOptions
{
    public const string SectionName = "MediaService";

    public required string MediaConvertRole { get; init; }

    public required string MediaConvertEndpoint { get; init; }
}
