using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ComfyBot.Common.Idioms;

namespace ComfyBot.Common.Http;

[ExcludeFromCodeCoverage]
public class HttpService : Singleton<IHttpService, HttpService>, IHttpService
{
    private readonly HttpClient httpClient;

    public HttpService()
    {
        httpClient = new HttpClient();
    }

    public async Task<T> GetAsync<T>(string url)
    {
        string result = await httpClient.GetAsync(url).Result.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<T>(result);
    }
}