using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Gu.PaftaBulucu.Bot.Models;
using Gu.PaftaBulucu.Bot.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.S3;
using Amazon.SimpleSystemsManagement;
using Gu.PaftaBulucu.Business.Services;
using Gu.PaftaBulucu.Data.Repositories;
using Microsoft.Extensions.Configuration;


[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Gu.PaftaBulucu.Bot
{
    public class Function
    {
        public async Task<APIGatewayProxyResponse> FunctionHandlerAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            LambdaLogger.Log(request.Body);

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK
            };

            try
            {
                var webhookMessage = JsonConvert.DeserializeObject<WebhookMessage>(request.Body);

                if (webhookMessage == null)
                {
                    return response;
                }

                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();
                var botService = serviceProvider.GetService<IBotService>();

                if (webhookMessage.Message?.Location != null)
                {
                    await botService.AskScaleAsync(webhookMessage.Message.Chat.Id, webhookMessage.Message.Location);
                }

                if (webhookMessage.CallbackQuery != null && webhookMessage.CallbackQuery.Message.Text == "Pafta ölçeğini seçiniz:" && int.TryParse(webhookMessage.CallbackQuery.Data, out int scale))
                {
                    await botService.QuerySheetAsync(webhookMessage.CallbackQuery.Message.MessageId, webhookMessage.CallbackQuery.Id, webhookMessage.CallbackQuery.Message.Chat.Id, scale);
                }
            }
            catch (Exception ex)
            {
                LambdaLogger.Log("Error: " + ex.Message);
            }

            return response;
        }

        private void ConfigureServices(ServiceCollection services)
        {
            var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

            services.AddAWSService<IAmazonDynamoDB>(new Amazon.Extensions.NETCore.Setup.AWSOptions()
            {
                Region = Amazon.RegionEndpoint.EUWest1
            });
            services.AddAWSService<IAmazonSimpleSystemsManagement>(new Amazon.Extensions.NETCore.Setup.AWSOptions()
            {
                Region = Amazon.RegionEndpoint.EUWest1
            });
            services.AddAWSService<IAmazonS3>(new Amazon.Extensions.NETCore.Setup.AWSOptions()
            {
                Region = Amazon.RegionEndpoint.EUWest1
            });
            services.AddSingleton<IConfiguration>(configuration);
            services.AddTransient<IBotService, BotService>();
            services.AddTransient<ITelegramService, TelegramService>();
            services.AddTransient<ISheetService, SheetService>();
            services.AddTransient<ISheetRepository, SheetRepository>();
            services.AddTransient<IAmazonDynamoDbService, AmazonDynamoDbService>();
            services.AddTransient<IParameterService, ParameterService>();
        }
    }
}
