namespace TestingApp;

public class TestService
{
    private readonly TestClient _testClient;

    public TestService(TestClient testClient) => _testClient = testClient;

    public async Task<HttpRequestMessage> CallTestEndpointAsync() => await _testClient.CallTestEndpointAsync();
}
