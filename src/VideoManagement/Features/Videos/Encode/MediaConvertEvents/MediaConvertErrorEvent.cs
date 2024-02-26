namespace VideoManagement.Features.Videos.Encode.MediaConvertEvents;

public sealed record MediaConvertErrorEvent([property: JsonPropertyName("detail")] Detail Detail)
{
    private const string ErrorEventName = "ERROR";

    public bool IsVideoEncodingFailedEvent()
        => Detail.Status == ErrorEventName;
}

public record Detail(
    [property: JsonPropertyName("jobId")] string JobId,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("errorMessage")] string ErrorMessage);
