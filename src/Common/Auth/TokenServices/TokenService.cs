namespace Common.Auth.TokenServices;

public class TokenService : ITokenService
{
    private const string AccessTokenKey = "access-token";

    private readonly IMemoryCache _cache;
    private readonly ITokenClient _tokenClient;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IMemoryCache cache, ITokenClient tokenClient, ILogger<TokenService> logger)
    {
        _cache = cache;
        _tokenClient = tokenClient;
        _logger = logger;
    }

    public async Task<OperationResult<string>> GetTokenAsync()
    {
        string? token = await _cache.GetOrCreateAsync(AccessTokenKey, async entry =>
        {
            var result = await _tokenClient.RequestAccessTokenAsync();
            if (result.IsFailure)
            {
                _logger.LogError("Failed to get token: {Error}", result.Failure);
                return null;
            }

            entry.AbsoluteExpirationRelativeToNow =
                TimeSpan.FromSeconds(result.Data.ExpiresInSeconds);
            return result.Data.AccessToken;
        });

        return token is null ? TokenClientErrors.FailedToGetToken() : token;
    }
}
