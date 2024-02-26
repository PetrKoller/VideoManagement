namespace Common.Auth.TokenServices;

public static class TokenClientErrors
{
    public static Failure FailedToGetToken() => new("TokenClient.FailedToGetToken", "Failed to get token.");
}
