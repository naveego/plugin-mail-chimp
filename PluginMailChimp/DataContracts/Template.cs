using Newtonsoft.Json;

namespace PluginMailChimp.DataContracts
{
    public class Template
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }
        
        [JsonProperty("publish_name")]
        public string PublishName { get; set; }

        [JsonProperty("publish_code")]
        public string PublishCode { get; set; }
    }
}