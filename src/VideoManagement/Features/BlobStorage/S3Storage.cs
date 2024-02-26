namespace VideoManagement.Features.BlobStorage;

public sealed class S3Storage : IBlobStorage
{
    private readonly IAmazonS3 _client;
    private readonly BlobStorageOptions _options;

    public S3Storage(IAmazonS3 client, IOptions<BlobStorageOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public string GenerateUploadUrl(string blobName, double durationInMinutes = 30)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _options.BucketName,
            Key = blobName,
            Expires = DateTime.UtcNow.AddMinutes(durationInMinutes),
            Verb = HttpVerb.PUT,
        };

        return _client.GetPreSignedURL(request);
    }
}
