namespace TestingApp;

public sealed class TestClient
{
    private readonly HttpClient _httpClient;

    public TestClient(HttpClient httpClient) => _httpClient = httpClient;

    // Test client that uses AuthMessageHandler to append token to request
    public async Task<HttpRequestMessage> CallTestEndpointAsync()
    {
        var response = await _httpClient.GetAsync("test");
        return response.RequestMessage!;
    }
}
