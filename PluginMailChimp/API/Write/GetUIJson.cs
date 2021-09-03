using System.Collections.Generic;
using Newtonsoft.Json;

namespace PluginMailChimp.API.Write
{
    public static partial class Write
    {
        public static string GetUIJson()
        {
            var uiJsonObj = $@"{{
                ""ui:order"": [
                    ""Template""
                ]
            }}";
            
            return uiJsonObj;
        }
    }
}