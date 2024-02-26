namespace VideoManagement.Test.Integration.VideosEndpoints;

public static class TestHelpers
{
    public static string EncodeTestHeaderValue(TestHeaderValue testHeaderValue)
    {
        string json = JsonSerializer.Serialize(testHeaderValue);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }

    public static HttpRequestMessage CreatePostRequest<T>(string endpoint, T content, string[] permissions, Guid? userId = null)
    {
        var authHeaderValue = new TestHeaderValue(userId, permissions);
        var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue(TestAuthConstants.AuthenticationScheme, EncodeTestHeaderValue(authHeaderValue));
        request.Content = JsonContent.Create(content);

        return request;
    }

    public static HttpRequestMessage CreateGetRequest(string endpoint, string[] permissions, Guid? userId = null)
    {
        var authHeaderValue = new TestHeaderValue(userId, permissions);
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue(TestAuthConstants.AuthenticationScheme, EncodeTestHeaderValue(authHeaderValue));

        return request;
    }
}
