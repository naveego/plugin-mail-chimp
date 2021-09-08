using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Naveego.Sdk.Logging;
using Naveego.Sdk.Plugins;
using Newtonsoft.Json;
using PluginMailChimp.API.Factory;
using PluginMailChimp.API.Utility;
using PluginMailChimp.DataContracts;

namespace PluginMailChimp.API.Write
{
    public static partial class Write
    {
        private static readonly SemaphoreSlim WriteSemaphoreSlim = new SemaphoreSlim(1, 1);

        public static async Task<string> WriteRecordAsync(IApiClient apiClient, Schema schema, Record record,
            IServerStreamWriter<RecordAck> responseStream)
        {
            // debug
            Logger.Debug($"Starting timer for {record.RecordId}");
            var timer = Stopwatch.StartNew();

            try
            {
                // debug
                Logger.Debug(JsonConvert.SerializeObject(record, Formatting.Indented));

                // semaphore
                await WriteSemaphoreSlim.WaitAsync();

                // get record map
                var recordMap = JsonConvert.DeserializeObject<Dictionary<string, object>>(record.DataJson);
                
                // write records
                // prepare url
                var url = "/messages/send-template";
                
                // prepare body
                var bodyVars = new List<ContentObject>();
                var bodyProperties = schema.Properties.Where(p => p.Id.StartsWith(Constants.BodyPropertyPrefix));
                foreach (var property in bodyProperties)
                {
                    var trimmedProperty = FindParamsRegex.Match(property.Id).Captures.First().Value;
                    trimmedProperty = trimmedProperty.TrimStart('{');
                    trimmedProperty = trimmedProperty.TrimEnd('}');
                    
                    try
                    {
                        if (recordMap.ContainsKey(property.Id))
                        {
                            bodyVars.Add(new ContentObject
                            {
                                Name = trimmedProperty,
                                Content = recordMap[property.Id].ToString()
                            });
                        }
                        else
                        {
                            bodyVars.Add(new ContentObject
                            {
                                Name = trimmedProperty,
                                Content = ""
                            });
                        }
                    }
                    catch
                    {
                        bodyVars.Add(new ContentObject
                        {
                            Name = trimmedProperty,
                            Content = ""
                        });
                    }
                }

                var body = new SendTemplateBody
                {
                    ApiKey = await apiClient.GetApiKey(),
                    TemplateName = schema.Query,
                    TemplateContent = new List<ContentObject>(),
                    Async = false,
                    Message = new MessageObject
                    {
                        To = new List<EmailObject>
                        {
                            new EmailObject
                            {
                                Email = recordMap[Constants.ToEmailId].ToString(),
                                Name = recordMap[Constants.ToEmailNameId].ToString(),
                                Type = "to"
                            }
                        },
                        MergeVariables = new List<MergeVariablesObject>
                        {
                            new MergeVariablesObject
                            {
                                Recipient = recordMap[Constants.ToEmailId].ToString(),
                                Variables = bodyVars
                            }
                        }
                    }
                };
                var bodyJson = JsonConvert.SerializeObject(body);
                var json = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8,
                    "application/json");
                
                var response = await apiClient.PostAsync(url, json);

                // send ack
                var ack = new RecordAck
                {
                    CorrelationId = record.CorrelationId,
                    Error = ""
                };
                
                if (!response.IsSuccessStatusCode)
                {
                    var apiError = JsonConvert.DeserializeObject<ApiError>(await response.Content.ReadAsStringAsync());
                    ack.Error = apiError.Message;
                }
                
                await responseStream.WriteAsync(ack);

                timer.Stop();
                Logger.Debug($"Acknowledged Record {record.RecordId} time: {timer.ElapsedMilliseconds}");

                return ack.Error;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error writing record {e.Message}");
                // send ack
                var ack = new RecordAck
                {
                    CorrelationId = record.CorrelationId,
                    Error = e.Message
                };
                await responseStream.WriteAsync(ack);

                timer.Stop();
                Logger.Debug($"Failed Record {record.RecordId} time: {timer.ElapsedMilliseconds}");

                return e.Message;
            }
            finally
            {
                WriteSemaphoreSlim.Release();
            }
        }
    }
}