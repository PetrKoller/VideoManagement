namespace Common.Auth.TokenServices;

/// <summary>
///    Represents a client that can get a token.
/// </summary>
public interface ITokenClient
{
    /// <summary>
    ///    Gets a token.
    /// </summary>
    /// <returns>Access token.</returns>
    Task<OperationResult<TokenResponse>> RequestAccessTokenAsync();
}
