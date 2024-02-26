namespace VideoManagement.Options;

public sealed class BlobStorageOptions
{
    public const string SectionName = "BlobStorage";

    public required string BucketName { get; init; }
}
