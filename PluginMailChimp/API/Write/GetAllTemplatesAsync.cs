using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PluginMailChimp.API.Factory;
using PluginMailChimp.DataContracts;

namespace PluginMailChimp.API.Write
{
    public static partial class Write
    {
        public static async Task<List<Template>> GetAllTemplatesAsync(IApiClient apiClient)
        {
            var requestBody = new RequestBody
            {
                ApiKey = await apiClient.GetApiKey()
            };

            var json = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await apiClient.PostAsync("/templates/list", json);

            var templates = JsonConvert.DeserializeObject<List<Template>>(await response.Content.ReadAsStringAsync());

            return templates;
        }
    }
}