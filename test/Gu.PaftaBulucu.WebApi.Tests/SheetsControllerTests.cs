using System.IO;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Newtonsoft.Json;
using Xunit;

namespace Gu.PaftaBulucu.WebApi.Tests
{
    public class SheetsControllerTests
    {


        [Fact]
        public async Task TestGetByName()
        {
            var lambdaFunction = new LambdaEntryPoint();

            var requestStr = File.ReadAllText("./SampleRequests/SheetsController-GetByName.json");
            var request = JsonConvert.DeserializeObject<APIGatewayProxyRequest>(requestStr);
            var context = new TestLambdaContext();
            var response = await lambdaFunction.FunctionHandlerAsync(request, context);

            Assert.Equal(200, response.StatusCode);
            Assert.Equal("{\"name\":\"Kırşehir-I 31-a-20-d-4-a\",\"lat\":39.80625,\"lon\":33.7,\"scale\":1}", response.Body);
        }
        
        [Fact]
        public async Task TestGetByCoordindates()
        {
            var lambdaFunction = new LambdaEntryPoint();

            var requestStr = File.ReadAllText("./SampleRequests/SheetsController-GetByCoordinates.json");
            var request = JsonConvert.DeserializeObject<APIGatewayProxyRequest>(requestStr);
            var context = new TestLambdaContext();
            var response = await lambdaFunction.FunctionHandlerAsync(request, context);

            Assert.Equal(200, response.StatusCode);
            Assert.Equal("[{\"name\":\"Mardin-N 47-b-17\",\"lat\":37.3,\"lon\":41.8,\"scale\":10}]", response.Body);
        }


    }
}
