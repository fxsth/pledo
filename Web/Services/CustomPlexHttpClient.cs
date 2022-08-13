using Plex.ServerApi.Api;

namespace Web.Services;

public class CustomPlexHttpClient : IPlexRequestsHttpClient
{
    private readonly HttpClient _httpClient;

    //public CustomPlexHttpClient(HttpClient httpClient)
    //{
    //    _httpClient = httpClient;
    //}
    public CustomPlexHttpClient()
    {
        _httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(30) };
    }

    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        return _httpClient.SendAsync(request);
    }
}