using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.SimpleSystemsManagement;
using Gu.PaftaBulucu.Business.Dtos;
using Gu.PaftaBulucu.Business.Services;
using Gu.PaftaBulucu.Business.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;


[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Gu.MailChimp.Api
{
    public class Function
    {
        public async Task<APIGatewayProxyResponse> FunctionHandlerAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            LambdaLogger.Log(request.Body);

            try
            {
                var addSubscriberDto = JsonConvert.DeserializeObject<AddSubscriberDto>(request.Body);

                if (addSubscriberDto == null || string.IsNullOrEmpty(addSubscriberDto.Email) || string.IsNullOrEmpty(addSubscriberDto.ListId))
                {
                    return CreateResponse(HttpStatusCode.BadRequest);
                }

                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();
                var mailChimpService = serviceProvider.GetService<IMailChimpService>();

                if (!RegexUtilities.IsValidEmail(addSubscriberDto.Email))
                {
                    return CreateResponse(HttpStatusCode.BadRequest, "Giriş yapılan e-posta adresi hatalı veya eksik.");
                }

                var result = await mailChimpService.AddMemberAsync(addSubscriberDto);

                if (result)
                {
                    return CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                LambdaLogger.Log("Error: " + ex.Message);
            }

            return CreateResponse(HttpStatusCode.InternalServerError);
        }

        private APIGatewayProxyResponse CreateResponse(HttpStatusCode statusCode, string message = null)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)statusCode,
                Body = message
            };
        }

        private void ConfigureServices(ServiceCollection services)
        {
            var configuration = new ConfigurationBuilder().AddSystemsManager("/gu/api/").Build();
            services.AddAWSService<IAmazonSimpleSystemsManagement>(new Amazon.Extensions.NETCore.Setup.AWSOptions()
            {
                Region = Amazon.RegionEndpoint.EUWest1
            });
            services.AddLogging();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddTransient<IMailChimpService, MailChimpService>();
        }
    }
}
