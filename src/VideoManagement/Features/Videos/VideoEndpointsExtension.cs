namespace VideoManagement.Features.Videos;

public static class VideoEndpointsExtension
{
    public static WebApplication MapVideoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(WebConstants.VideosPath);

        _ = group.MapPost("start-upload", StartUpload)
            .RequireAuthorization(Permissions.UploadVideo);

        _ = group.MapGet("{id:guid}/download-link", GetDownloadLink)
            .RequireAuthorization(Permissions.DownloadVideo);

        _ = group.MapGet("{id:guid}/signed-resource", GetStreamLink)
            .RequireAuthorization(Permissions.PlayVideo);

        return app;
    }

    private static async Task<Results<Ok<VideoUploadResponse>, BadRequest<Failure>>> StartUpload(
        StartVideoUpload videoUpload,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new StartVideoUploadCommand(videoUpload), cancellationToken);
        return result.Match<Results<Ok<VideoUploadResponse>, BadRequest<Failure>>>(
            val => TypedResults.Ok(val),
            failure => TypedResults.BadRequest(failure));
    }

    private static async Task<Results<Ok<string>, NotFound<Failure>>> GetDownloadLink(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetDownloadLinkRequest(id), cancellationToken);
        return result.Match<Results<Ok<string>, NotFound<Failure>>>(
            val => TypedResults.Ok(val),
            failure => TypedResults.NotFound(failure));
    }

    private static async Task<Results<Ok<SignedResource>, NotFound<Failure>>> GetStreamLink(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetStreamLinkRequest(id), cancellationToken);
        return result.Match<Results<Ok<SignedResource>, NotFound<Failure>>>(
            value => TypedResults.Ok(value),
            failure => TypedResults.NotFound(failure));
    }
}
