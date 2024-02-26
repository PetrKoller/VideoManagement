namespace VideoManagement.Features.Videos.Upload;

public sealed record S3Event(List<Record> Records)
{
    private const string PutEventName = "ObjectCreated:Put";

    public bool IsVideoUploadedEvent()
        => Records.Count != 0 && Records.First().EventName == PutEventName;

    public string GetVideoKey()
        => Records.First().S3.Object.Key;
}

public sealed record Record(
    [property: JsonPropertyName("eventName")] string EventName,
    [property: JsonPropertyName("s3")] S3Info S3);

public sealed record S3Info(
    [property: JsonPropertyName("object")] S3Object Object);

public sealed record S3Object(
    [property: JsonPropertyName("key")] string Key);
