namespace Common.Auth.TokenServices;

/// <summary>
///    Represents a service that can get a token.
/// </summary>
public interface ITokenService
{
    /// <summary>
    ///    Gets a token.
    /// </summary>
    /// <returns>Async operation result with access token.</returns>
    Task<OperationResult<string>> GetTokenAsync();
}
