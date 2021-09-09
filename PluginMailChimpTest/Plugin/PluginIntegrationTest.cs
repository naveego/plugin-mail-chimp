using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Naveego.Sdk.Plugins;
using Newtonsoft.Json;
using PluginMailChimp.API.Utility;
using PluginMailChimp.DataContracts;
using PluginMailChimp.Helper;
using Xunit;
using Record = Naveego.Sdk.Plugins.Record;

namespace PluginMailChimpTest.Plugin
{
    public class PluginIntegrationTest
    {
        private Settings GetSettings()
        {

            return new Settings
            {
                ApiKey = "" // add to test
            };
        }

        private ConnectRequest GetConnectSettings()
        {
            var settings = GetSettings();

            return new ConnectRequest
            {
                SettingsJson = JsonConvert.SerializeObject(settings),
                OauthConfiguration = null,
                OauthStateJson = ""
            };
        }

        private ConfigureWriteFormData GetFormData(string templateName = "Transactional Template Test")
        {
            return new ConfigureWriteFormData
            {
                Template = templateName
            };
        }

         [Fact]
         public async Task ConnectSessionTest()
         {
             // setup
             Server server = new Server
             {
                 Services = {Publisher.BindService(new PluginMailChimp.Plugin.Plugin())},
                 Ports = {new ServerPort("localhost", 0, ServerCredentials.Insecure)}
             };
             server.Start();

             var port = server.Ports.First().BoundPort;

             var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
             var client = new Publisher.PublisherClient(channel);

             var request = GetConnectSettings();
             var disconnectRequest = new DisconnectRequest();

             // act
             var response = client.ConnectSession(request);
             var responseStream = response.ResponseStream;
             var records = new List<ConnectResponse>();

             while (await responseStream.MoveNext())
             {
                 records.Add(responseStream.Current);
                 client.Disconnect(disconnectRequest);
             }

             // assert
             Assert.Single(records);

             // cleanup
             await channel.ShutdownAsync();
             await server.ShutdownAsync();
         }

         [Fact]
         public async Task ConnectTest()
         {
             // setup
             Server server = new Server
             {
                 Services = {Publisher.BindService(new PluginMailChimp.Plugin.Plugin())},
                 Ports = {new ServerPort("localhost", 0, ServerCredentials.Insecure)}
             };
             server.Start();

             var port = server.Ports.First().BoundPort;

             var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
             var client = new Publisher.PublisherClient(channel);

             var request = GetConnectSettings();

             // act
             var response = client.Connect(request);

             // assert
             Assert.IsType<ConnectResponse>(response);
             Assert.Equal("", response.SettingsError);
             Assert.Equal("", response.ConnectionError);
             Assert.Equal("", response.OauthError);

             // cleanup
             await channel.ShutdownAsync();
             await server.ShutdownAsync();
         }

         [Fact]
         public async Task DiscoverSchemasAllTest()
         {
             // setup
             Server server = new Server
             {
                 Services = {Publisher.BindService(new PluginMailChimp.Plugin.Plugin())},
                 Ports = {new ServerPort("localhost", 0, ServerCredentials.Insecure)}
             };
             server.Start();

             var port = server.Ports.First().BoundPort;

             var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
             var client = new Publisher.PublisherClient(channel);

             var connectRequest = GetConnectSettings();

             var request = new DiscoverSchemasRequest
             {
                 Mode = DiscoverSchemasRequest.Types.Mode.All,
                 SampleSize = 10
             };

             // act
             client.Connect(connectRequest);
             var response = client.DiscoverSchemas(request);

             // assert
             Assert.IsType<DiscoverSchemasResponse>(response);
             Assert.Empty(response.Schemas);

             // cleanup
             await channel.ShutdownAsync();
             await server.ShutdownAsync();
         }

         [Fact]
         public async Task DiscoverSchemasRefreshTest()
         {
             // setup
             Server server = new Server
             {
                 Services = {Publisher.BindService(new PluginMailChimp.Plugin.Plugin())},
                 Ports = {new ServerPort("localhost", 0, ServerCredentials.Insecure)}
             };
             server.Start();

             var port = server.Ports.First().BoundPort;

             var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
             var client = new Publisher.PublisherClient(channel);

             var connectRequest = GetConnectSettings();

             var request = new DiscoverSchemasRequest
             {
                 Mode = DiscoverSchemasRequest.Types.Mode.Refresh,
                 SampleSize = 10,
                 ToRefresh = { }
             };

             // act
             client.Connect(connectRequest);
             var response = client.DiscoverSchemas(request);

             // assert
             Assert.IsType<DiscoverSchemasResponse>(response);
             Assert.Empty(response.Schemas);

             // cleanup
             await channel.ShutdownAsync();
             await server.ShutdownAsync();
         }

         [Fact]
         public async Task ReadStreamTest()
         {
             // setup
             Server server = new Server
             {
                 Services = {Publisher.BindService(new PluginMailChimp.Plugin.Plugin())},
                 Ports = {new ServerPort("localhost", 0, ServerCredentials.Insecure)}
             };
             server.Start();

             var port = server.Ports.First().BoundPort;

             var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
             var client = new Publisher.PublisherClient(channel);

             var connectRequest = GetConnectSettings();
             
             var request = new ReadRequest()
             {
                 DataVersions = new DataVersions
                 {
                     JobId = "test"
                 },
                 JobId = "test",
             };

             // act
             client.Connect(connectRequest);

             var response = client.ReadStream(request);
             var responseStream = response.ResponseStream;
             var records = new List<Record>();

             while (await responseStream.MoveNext())
             {
                 records.Add(responseStream.Current);
             }

             // assert
             Assert.Empty(records);

             // cleanup
             await channel.ShutdownAsync();
             await server.ShutdownAsync();
         }

        [Fact]
        public async Task ConfigureWriteTest()
        {
            // setup
            Server server = new Server
            {
                Services = {Publisher.BindService(new PluginMailChimp.Plugin.Plugin())},
                Ports = {new ServerPort("localhost", 0, ServerCredentials.Insecure)}
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);
            
            var connectRequest = GetConnectSettings();
            
            // act
            client.Connect(connectRequest);
            
            var formData = GetFormData("Transactional Template Test");

            var request = new ConfigureWriteRequest
            {
                Form = new ConfigurationFormRequest
                {
                    DataJson = JsonConvert.SerializeObject(formData),
                    IsSave = true,
                    StateJson = "{}"
                }
            };
            
            var response =  client.ConfigureWrite(request);
            
            // assert
            Assert.IsType<ConfigureWriteResponse>(response);

            var schema = response.Schema;
            
            Assert.Equal("transactional-template", schema.Id);
            Assert.Equal("Transactional Template Test", schema.Name);
            Assert.Equal("", schema.Description);
            Assert.Equal("transactional-template", schema.Query);
            Assert.Equal(Schema.Types.DataFlowDirection.Write, schema.DataFlowDirection);
            Assert.Equal(9, schema.Properties.Count);

            var property = schema.Properties[0];
            
            Assert.Equal("TEMPLATE_{{subject}}", property.Id);
            Assert.Equal("TEMPLATE_{{subject}}", property.Name);
            Assert.Equal("", property.Description);
            Assert.Equal(PropertyType.String, property.Type);
            
            //cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }

        [Fact]
        public async Task WriteTest()
        {
            // setup
            Server server = new Server
            {
                Services = {Publisher.BindService(new PluginMailChimp.Plugin.Plugin())},
                Ports = {new ServerPort("localhost", 0, ServerCredentials.Insecure)}
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);
            
            var connectRequest = GetConnectSettings();
            
            var records = new List<Record>()
            {
                {
                    new Record
                    {
                        Action = Record.Types.Action.Upsert,
                        CorrelationId = "test",
                        RecordId = "record1",
                        DataJson = "{\"TEMPLATE_{{subject}}\":\"Test Subject\",\"TEMPLATE_{{referral_code}}\":\"r-code\",\"TEMPLATE_{{first_name}}\":\"Wyatt\",\"TEMPLATE_{{checking_url}}\":\"cool_url\",\"TEMPLATE_{{checking_name}}\":\"Super Duper Checking Account\",\"TEMPLATE_{{current_year}}\":\"2021\",\"ToEmail\":\"wyatt.roehler@aunalytics.com\",\"ToEmailName\":\"Wyatt Roehler\"}",
                    }
                }
            };
            
            var recordAcks = new List<RecordAck>();
            
            // act
            client.Connect(connectRequest);
            
            var formData = GetFormData("Transactional Template Test");

            var schemaRequest = new ConfigureWriteRequest
            {
                Form = new ConfigurationFormRequest
                {
                    DataJson = JsonConvert.SerializeObject(formData),
                    IsSave = true,
                    StateJson = "{}"
                }
            };
            
            var schemaResponse =  client.ConfigureWrite(schemaRequest);

            var prepareWriteRequest = new PrepareWriteRequest()
            {
                Schema = schemaResponse.Schema,
                CommitSlaSeconds = 1000,
                DataVersions = new DataVersions
                {
                    JobId = "jobUnitTest",
                    ShapeId = "shapeUnitTest",
                    JobDataVersion = 1,
                    ShapeDataVersion = 1
                }
            };
            client.PrepareWrite(prepareWriteRequest);
            
            using (var call = client.WriteStream())
            {
                var responseReaderTask = Task.Run(async () =>
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        var ack = call.ResponseStream.Current;
                        recordAcks.Add(ack);
                    }
                });

                foreach (Record record in records)
                {
                    await call.RequestStream.WriteAsync(record);
                }

                await call.RequestStream.CompleteAsync();
                await responseReaderTask;
            }

            // assert
            Assert.Single(recordAcks);
            Assert.Equal("", recordAcks[0].Error);
            Assert.Equal("test", recordAcks[0].CorrelationId);
            
            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }
    }
}