using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Gu.PaftaBulucu.WebApi
{
    public class LambdaEntryPoint : Amazon.Lambda.AspNetCoreServer.APIGatewayProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            builder
                .ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    configurationBuilder.AddSystemsManager("/gu/api/");
                })
                .UseStartup<Startup>();
        }

        protected override void Init(IHostBuilder builder)
        {
        }
    }
}
