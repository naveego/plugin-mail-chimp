using System.Threading.Tasks;

namespace PluginMailChimp.API.Factory
{
    public interface IApiAuthenticator
    {
        Task<string> GetToken();
    }
}