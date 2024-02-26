namespace Common.Test.Integration.Setup;

public sealed class TestHttpServer : IDisposable
{
    private WireMockServer _server = null!;

    public string Url => _server.Url!;

    public void Start() => _server = WireMockServer.Start();

    public void SetupValidTokenResponse()
    {
        _server.Given(Request.Create()
                .WithPath("/test")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithSuccess());

        _server.Given(Request.Create()
                .WithPath("/realms/PowerTrainer/protocol/openid-connect/token")
                .WithBody(new RegexMatcher($"client_id={TestConstants.ValidClientId}"))
                .UsingPost())
            .RespondWith(Response.Create()
                .WithBody(JsonSerializer.Serialize(TestConstants.TestTokenResponse))
                .WithSuccess());
    }

    public void SetupInvalidTokenResponse()
    {
        _server.Given(Request.Create()
                .WithPath("/test")
                .UsingGet())
            .RespondWith(Response.Create()
                .WithSuccess());

        _server.Given(Request.Create()
                .WithPath("/realms/PowerTrainer/protocol/openid-connect/token")
                .WithBody(new RegexMatcher($"client_id={TestConstants.ValidClientId}"))
                .UsingPost())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.Unauthorized));
    }

    public void Reset() => _server.Reset();

    public void Dispose()
    {
        _server.Stop();
        _server.Dispose();
    }
}
