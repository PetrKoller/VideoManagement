namespace VideoManagement.Features.Videos.Stream;

public sealed record GetStreamLinkRequest(Guid VideoId) : IRequest<OperationResult<SignedResource>>;

public sealed class GetStreamLinkRequestHandler : IRequestHandler<GetStreamLinkRequest, OperationResult<SignedResource>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IResourceSigner _resourceSigner;

    public GetStreamLinkRequestHandler(
        ISqlConnectionFactory sqlConnectionFactory,
        IResourceSigner resourceSigner)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _resourceSigner = resourceSigner;
    }

    public async ValueTask<OperationResult<SignedResource>> Handle(GetStreamLinkRequest request, CancellationToken cancellationToken)
    {
        using var connection = await _sqlConnectionFactory.CreateConnectionAsync(cancellationToken);
        const string sql = """
                           SELECT stream_file_location as StreamFileLocation, destination_location as DestinationLocation
                           FROM videos
                           WHERE external_id = @VideoId
                           AND status = @Status
                           """;

        var videoInfo = await connection.QuerySingleOrDefaultAsync<VideoInfo>(
            sql,
            new { request.VideoId, Status = VideoStatus.Completed });

        return videoInfo is null ?
            VideoErrors.NotFound :
            _resourceSigner.CreateSignedResource($"{videoInfo.DestinationLocation}/*", videoInfo.StreamFileLocation);
    }
}
