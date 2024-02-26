namespace VideoManagement.Options;

public sealed class SqsOptions
{
    public const string SectionName = "Sqs";

    public required string VideoUploadedQueue { get; init; }

    public required string EncodingFailedQueue { get; init; }

    public required string EncodingCompletedQueue { get; init; }

    public required int WaitTimeSeconds { get; init; }
}
