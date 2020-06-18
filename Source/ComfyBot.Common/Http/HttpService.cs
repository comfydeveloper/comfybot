namespace ComfyBot.Common.Http
{
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    using ComfyBot.Common.Idioms;

    [ExcludeFromCodeCoverage]
    public class HttpService : Singleton<IHttpService, HttpService>, IHttpService
    {
        private readonly HttpClient httpClient;

        public HttpService()
        {
            this.httpClient = new HttpClient();
        }

        public async Task<T> GetAsync<T>(string url)
        {
            string result = await this.httpClient.GetAsync(url).Result.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(result);
        }
    }
}