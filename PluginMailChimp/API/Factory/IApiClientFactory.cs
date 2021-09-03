using PluginMailChimp.DataContracts;
using PluginMailChimp.Helper;

namespace PluginMailChimp.API.Factory
{
    public interface IApiClientFactory
    {
        IApiClient CreateApiClient(Settings settings);
    }
}