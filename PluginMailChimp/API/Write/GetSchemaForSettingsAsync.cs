using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Naveego.Sdk.Plugins;
using Newtonsoft.Json;
using PluginMailChimp.API.Utility;
using PluginMailChimp.DataContracts;

namespace PluginMailChimp.API.Write
{
    public static partial class Write
    {
        private static Regex FindParamsRegex = new Regex(@"\{\{(\w+)\}\}");

        public static async Task<Schema> GetSchemaForSettingsAsync(ConfigureWriteFormData formData, List<Template> templates)
        {
            var template = templates.First(x => x.PublishName == formData.Template);
            
            var schema = new Schema
            {
                Id = template.Slug,
                Name = template.PublishName,
                Description = "",
                DataFlowDirection = Schema.Types.DataFlowDirection.Write,
                Query = template.Slug,
            };

            var templateParams = FindParamsRegex.Matches(template.PublishCode);

            foreach (var match in templateParams)
            {
                var property = new Property
                {
                    Id = $"{Constants.BodyPropertyPrefix}_{match}",
                    Name = $"{Constants.BodyPropertyPrefix}_{match}",
                    Description = "",
                    Type = PropertyType.String,
                    TypeAtSource = "",
                };
            
                schema.Properties.Add(property);
            }
            
            var emailProperty = new Property
            {
                Id = Constants.ToEmailId,
                Name = Constants.ToEmailIdName,
                Description = "",
                Type = PropertyType.String,
                TypeAtSource = "",
            };
            schema.Properties.Add(emailProperty);
            
            var emailNameProperty = new Property
            {
                Id = Constants.ToEmailNameId,
                Name = Constants.ToEmailNameName,
                Description = "",
                Type = PropertyType.String,
                TypeAtSource = "",
            };
            schema.Properties.Add(emailNameProperty);

            return schema;
        }
    }
}