namespace ComfyBot.Common.Http
{
    using System.Threading.Tasks;

    public interface IHttpService
    {
        Task<T> GetAsync<T>(string url);
    }
}