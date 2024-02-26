namespace Common.Auth.TokenServices;

public sealed class KeycloakCredentialsClient : ITokenClient
{
    private const string TokenEndpoint = "protocol/openid-connect/token";
    private const string ClientCredentialsGrantType = "client_credentials";
    private const string Realms = "realms";

    private readonly JwtSettings _jwtSettings;
    private readonly HttpClient _httpClient;

    public KeycloakCredentialsClient(IOptions<JwtSettings> jwtSettings, HttpClient httpClient)
    {
        _jwtSettings = jwtSettings.Value;
        _httpClient = httpClient;
    }

    public async Task<OperationResult<TokenResponse>> RequestAccessTokenAsync()
    {
        var values = new Dictionary<string, string>
        {
            ["grant_type"] = ClientCredentialsGrantType,
            ["client_id"] = _jwtSettings.ClientId,
            ["client_secret"] = _jwtSettings.ClientSecret,
        };

        var content = new FormUrlEncodedContent(values);
        var response = await _httpClient.PostAsync($"{Realms}/{_jwtSettings.Realm}/{TokenEndpoint}", content);

        if (!response.IsSuccessStatusCode)
        {
            return TokenClientErrors.FailedToGetToken();
        }

        var data = await response.Content.ReadFromJsonAsync<TokenResponse>();

        return data is null ? TokenClientErrors.FailedToGetToken() : data;
    }
}
