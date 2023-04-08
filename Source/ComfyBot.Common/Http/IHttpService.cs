using System.Threading.Tasks;

namespace ComfyBot.Common.Http;

public interface IHttpService
{
    Task<T> GetAsync<T>(string url);
}