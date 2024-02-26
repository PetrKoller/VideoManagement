namespace Common.Test.Integration.Setup;

public static class TestConstants
{
    public const string ValidClientId = "valid-client-id";
    public const string InvalidClientId = "invalid-client-id";

    public static readonly TokenResponse TestTokenResponse = new(
        "test-access-token",
        "test-refresh-token",
        "test-id-token",
        300,
        300);
}
