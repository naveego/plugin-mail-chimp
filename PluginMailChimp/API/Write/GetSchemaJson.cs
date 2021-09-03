using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PluginMailChimp.API.Utility;
using PluginMailChimp.DataContracts;

namespace PluginMailChimp.API.Write
{
    public static partial class Write
    {
        public static string GetSchemaJson(List<Template> templates)
        {
            var schemaJsonObj = $@"{{
            ""type"": ""object"",
            ""properties"": {{
                ""Template"": {{
                    ""type"": ""string"",
                    ""title"": ""Template"",
                    ""description"": ""The template to use"",
                    ""enum"": [
                        {string.Join(',', templates.Select(x => $@"""{x.PublishName}"""))}
                    ]
                }}
            }},
            ""required"": [""Template""]
        }}";

            return schemaJsonObj;
        }
    }
}