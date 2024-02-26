namespace Common.Auth.Handlers;

/// <summary>
///    Represents a message handler that adds an authorization header to the request.
/// </summary>
public sealed class AuthMessageHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthMessageHandler> _logger;

    public AuthMessageHandler(ITokenService tokenService, ILogger<AuthMessageHandler> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Consider using Polly for retries, if failures occur often. And maybe create Retry Handler that will use polly and will be general for all http clients. IT is like middleware for http clients.
        var tokenResult = await _tokenService.GetTokenAsync();
        if (tokenResult.IsFailure)
        {
            _logger.LogError("Failed to get token: {Error}", tokenResult.Failure);
            throw new HttpRequestException("Unable to retrieve token.");
        }

        request.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, tokenResult.Data);

        return await base.SendAsync(request, cancellationToken);
    }
}
