namespace VideoManagement.Contracts.Api.V1;

/// <summary>
/// Client for video management service.
/// </summary>
public interface IVideoClient
{
    /// <summary>
    /// Starts a video upload and returns the signed url to which the client should upload the video.
    /// Along with the signed url, the response also contains the id of the video that should be used
    /// for further communication with video management service.
    /// </summary>
    /// <param name="upload"><see cref="StartVideoUpload"/> options to start the process.</param>
    /// <returns><see cref="VideoUploadResponse"/> if starting the process was successful, otherwise failure.</returns>
    Task<OperationResult<VideoUploadResponse>> StartUpload(StartVideoUpload upload);

    /// <summary>
    /// Gets the signed resource containing a streaming url and mandatory information for constructing
    /// signed cookies for the client.
    /// </summary>
    /// <example>
    /// In order for client to successfully stream the video from the returned url,
    /// the following cookies must be set on the response:
    /// <code>
    /// <![CDATA[
    /// var options = new CookieOptions
    /// {
    ///     Secure = true,
    ///     HttpOnly = true,
    /// };
    ///
    /// context.Response.Cookies.Append("CloudFront-Policy", resource.Policy, options);
    /// context.Response.Cookies.Append("CloudFront-Signature", resource.Signature, options);
    /// context.Response.Cookies.Append("CloudFront-Key-Pair-Id", resource.KeyPairId, options);
    ///
    /// return Results.Ok(resource.Url);
    /// ]]>
    /// </code>
    /// </example>
    /// <param name="videoId">Id of the video for which the signed resource should be returned.</param>
    /// <returns><see cref="SignedResource"/> that contains all necessary information.</returns>
    Task<OperationResult<SignedResource>> GetSignedResource(Guid videoId);

    /// <summary>
    /// Returns the url from which the video can be downloaded if the video is downloadable.
    /// </summary>
    /// <param name="videoId">Id of the video.</param>
    /// <returns>Download url if video exists and is downloadable, otherwise failure.</returns>
    Task<OperationResult<string>> GetDownloadUrl(Guid videoId);
}
