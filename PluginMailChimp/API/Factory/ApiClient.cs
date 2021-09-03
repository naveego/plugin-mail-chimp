using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Naveego.Sdk.Logging;
using Newtonsoft.Json;
using PluginMailChimp.DataContracts;
using PluginMailChimp.API.Utility;
using PluginMailChimp.Helper;

namespace PluginMailChimp.API.Factory
{
    public class ApiClient : IApiClient
    {
        private IApiAuthenticator Authenticator { get; set; }
        private static HttpClient Client { get; set; }
        private Settings Settings { get; set; }

        public ApiClient(HttpClient client, Settings settings)
        {
            Authenticator = new ApiAuthenticator(client, settings);
            Client = client;
            Settings = settings;

            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task TestConnection()
        {
            try
            {
                var uriBuilder =
                    new UriBuilder(
                        $"{Constants.BaseApiUrl.TrimEnd('/')}/{Constants.TestConnectionPath.TrimStart('/')}");
                var uri = new Uri(uriBuilder.ToString());

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = uri,
                    Content = new StringContent(JsonConvert.SerializeObject(
                            new RequestBody
                            {
                                ApiKey = await Authenticator.GetToken()
                            }
                        ), 
                        Encoding.UTF8, 
                        "application/json"
                    )
                };

                var response = await Client.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
        }

        public async Task<string> GetApiKey()
        {
            return await Authenticator.GetToken();
        }

        public async Task<HttpResponseMessage> GetAsync(string path)
        {
            try
            {
                var uriBuilder =
                    new UriBuilder(
                        $"{Constants.BaseApiUrl.TrimEnd('/')}/{path.TrimStart('/')}");

                var uri = new Uri(uriBuilder.ToString());

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = uri,
                };

                return await Client.SendAsync(request);
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> PostAsync(string path, StringContent json)
        {
            try
            {
                var uriBuilder =
                    new UriBuilder(
                        $"{Constants.BaseApiUrl.TrimEnd('/')}/{path.TrimStart('/')}");

                var uri = new Uri(uriBuilder.ToString());

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = uri,
                    Content = json
                };

                return await Client.SendAsync(request);
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> PutAsync(string path, StringContent json)
        {
            try
            {
                var uriBuilder =
                    new UriBuilder(
                        $"{Constants.BaseApiUrl.TrimEnd('/')}/{path.TrimStart('/')}");

                var uri = new Uri(uriBuilder.ToString());

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = uri,
                    Content = json
                };

                return await Client.SendAsync(request);
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> PatchAsync(string path, StringContent json)
        {
            try
            {
                var uriBuilder =
                    new UriBuilder(
                        $"{Constants.BaseApiUrl.TrimEnd('/')}/{path.TrimStart('/')}");

                var uri = new Uri(uriBuilder.ToString());

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Patch,
                    RequestUri = uri,
                    Content = json
                };

                return await Client.SendAsync(request);
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> DeleteAsync(string path)
        {
            try
            {
                var uriBuilder =
                    new UriBuilder(
                        $"{Constants.BaseApiUrl.TrimEnd('/')}/{path.TrimStart('/')}");

                var uri = new Uri(uriBuilder.ToString());

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = uri
                };

                return await Client.SendAsync(request);
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
        }
    }
}