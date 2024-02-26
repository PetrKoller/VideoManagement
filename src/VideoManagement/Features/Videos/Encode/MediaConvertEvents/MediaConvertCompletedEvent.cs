namespace VideoManagement.Features.Videos.Encode.MediaConvertEvents;

public sealed record MediaConvertCompletedEvent([property: JsonPropertyName("detail")] CompletedDetail Detail)
{
    private const string CompletedEventName = "COMPLETE";
    public const string HlsGroupName = "HLS_GROUP";
    public const string FileGroupName = "FILE_GROUP";

    public bool IsVideoEncodingCompletedEvent()
        => Detail.Status == CompletedEventName;
}

public record CompletedDetail(
    [property: JsonPropertyName("jobId")] string JobId,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("outputGroupDetails")] List<OutputGroupDetail> OutputGroupDetails);

public sealed record OutputGroupDetail(
    [property: JsonPropertyName("outputDetails")] List<OutputDetail> OutputDetails,
    [property: JsonPropertyName("type")] string Type);

public sealed record OutputDetail([property: JsonPropertyName("outputFilePaths")] List<string> OutputFilePaths);
