namespace VideoManagement.Features.Videos.Download;

public sealed record GetDownloadLinkRequest(Guid VideoId) : IRequest<OperationResult<string>>;

public sealed class GetDownloadLinkRequestHandler : IRequestHandler<GetDownloadLinkRequest, OperationResult<string>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IResourceSigner _resourceSigner;

    public GetDownloadLinkRequestHandler(
        ISqlConnectionFactory sqlConnectionFactory,
        IResourceSigner resourceSigner)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _resourceSigner = resourceSigner;
    }

    public async ValueTask<OperationResult<string>> Handle(GetDownloadLinkRequest request, CancellationToken cancellationToken)
    {
        using var connection = await _sqlConnectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = "SELECT download_file_location FROM videos WHERE external_id = @VideoId AND is_downloadable = true";

        string? downloadFileLocation = await connection.QuerySingleOrDefaultAsync<string>(sql, new { request.VideoId });

        if (downloadFileLocation is null)
        {
            return VideoErrors.NotFound;
        }

        var signedResource = _resourceSigner.CreateSignedResource(downloadFileLocation);

        return $"{signedResource.Url}?Policy={signedResource.Policy}&Signature={signedResource.Signature}&Key-Pair-Id={signedResource.KeyPairId}";
    }
}
