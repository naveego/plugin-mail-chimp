using System.Net.Http;
using PluginMailChimp.DataContracts;
using PluginMailChimp.Helper;

namespace PluginMailChimp.API.Factory
{
    public class ApiClientFactory: IApiClientFactory
    {
        private HttpClient Client { get; set; }

        public ApiClientFactory(HttpClient client)
        {
            Client = client;
        }

        public IApiClient CreateApiClient(Settings settings)
        {
            return new ApiClient(Client, settings);
        }
    }
}