namespace VideoManagement.Options;

public sealed class CloudFrontOptions
{
    public const string SectionName = "CloudFront";

    public required string Url { get; init; }

    public required string PublicKeyId { get; init; }

    public required string PrivateKeyLocation { get; init; }

    public required int LinkLifetimeInMinutes { get; init; }
}
