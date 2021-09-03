using System.Collections.Generic;
using Newtonsoft.Json;

namespace PluginMailChimp.DataContracts
{
    public class RequestBody
    {
        [JsonProperty("key", Required = Required.Always)]
        public string ApiKey { get; set; }
    }

    public class SendTemplateBody : RequestBody
    {
        [JsonProperty("template_name", Required = Required.Always)]
        public string TemplateName { get; set; }
        
        [JsonProperty("template_content", Required = Required.Always)]
        public List<ContentObject> TemplateContent { get; set; }
        
        [JsonProperty("message", Required = Required.Always)]
        public MessageObject Message { get; set; }
        
        [JsonProperty("async", Required = Required.Always)]
        public bool Async { get; set; }
    }

    public class MessageObject
    {
        [JsonProperty("to", Required = Required.Always)]
        public List<EmailObject> To { get; set; }
        
        [JsonProperty("merge_vars", Required = Required.Always)]
        public List<MergeVariablesObject> MergeVariables { get; set; }
    }

    public class EmailObject
    {
        [JsonProperty("email", Required = Required.Always)]
        public string Email { get; set; }
        
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }
        
        [JsonProperty("type", Required = Required.Always)]
        public string Type { get; set; }
    }

    public class MergeVariablesObject
    {
        [JsonProperty("rcpt", Required = Required.Always)]
        public string Recipient { get; set; }
        
        [JsonProperty("vars", Required = Required.Always)]
        public List<ContentObject> Variables { get; set; }
    }
    
    public class ContentObject
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }
        
        [JsonProperty("content", Required = Required.Always)]
        public string Content { get; set; }
    }
}