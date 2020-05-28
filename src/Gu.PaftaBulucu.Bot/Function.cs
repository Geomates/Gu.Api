using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Gu.PaftaBulucu.Bot.Models;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Gu.PaftaBulucu.Bot
{
    public class Functions
    {

        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest request, ILambdaContext context)
        {
            LambdaLogger.Log(request.Body);

            var response = new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK
            };

            WebhookMessage webhookMessage = null;

            try
            {
                webhookMessage = JsonConvert.DeserializeObject<WebhookMessage>(request.Body);
            }
            catch (Exception ex)
            {
                LambdaLogger.Log("Error: " + ex.Message);
            }

            if (webhookMessage == null)
            {
                return response;
            }

            if (webhookMessage.Message?.Location != null)
            {
                await botService.SetLocationAsync(webhookMessage.Message.Chat.Id, webhookMessage.Message.Location);
            }

            if (webhookMessage.CallbackQuery != null)
            {

                switch (webhookMessage.CallbackQuery.Message.Text)
                {
                    case BotDialog.ASK_MAGNITUDE:
                        if (double.TryParse(webhookMessage.CallbackQuery.Data, out double magnitude))
                        {
                            await botService.SetMagnitudeAsync(webhookMessage.CallbackQuery.Message.MessageId, webhookMessage.CallbackQuery.Id, webhookMessage.CallbackQuery.Message.Chat.Id, magnitude);
                        }
                        break;
                }
            }

            //var response = new APIGatewayProxyResponse
            //{
            //    StatusCode = (int)HttpStatusCode.OK,
            //    Body = "Hello AWS Serverless",
            //    Headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } }
            //};

            return response;
        }
    }
}
