using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace Gu.ApiGateway.CustomAuthorizer
{
    public class Function
    {
        public async Task<APIGatewayCustomAuthorizerResponse> FunctionHandlerAsync(APIGatewayCustomAuthorizerRequest request, ILambdaContext context)
        {
            var effect = "Deny";
            var principalId = string.Empty;
            try
            {
                IConfigurationManager<OpenIdConnectConfiguration> configurationManager =
                    new ConfigurationManager<OpenIdConnectConfiguration>(
                        Environment.GetEnvironmentVariable("METADATA_ADDRESS"),
                        new OpenIdConnectConfigurationRetriever());

                var openIdConfig = await configurationManager.GetConfigurationAsync(CancellationToken.None);


                var validationParameters =
                    new TokenValidationParameters
                    {
                        ValidIssuer = Environment.GetEnvironmentVariable("VALID_ISSUER"),
                        ValidAudience = Environment.GetEnvironmentVariable("VALID_AUDIENCE"),
                        IssuerSigningKeys = openIdConfig.SigningKeys,
                        
                    };

                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

                var token = request.AuthorizationToken.Substring("Bearer ".Length);
                var claimsPrincipal = handler.ValidateToken(token, validationParameters, out _);

                principalId = claimsPrincipal?.FindFirst(ClaimTypes.Email).Value;
                effect = "Allow";
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                Console.WriteLine("StackTrace: " + e.StackTrace);
            }

            Console.WriteLine("MethodArn: " + request.MethodArn);

            return new APIGatewayCustomAuthorizerResponse
            {
                PrincipalID = principalId,
                PolicyDocument = new APIGatewayCustomAuthorizerPolicy
                {
                    Version = "2012-10-17",
                    Statement = new List<APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement>() {
                        new APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement
                        {
                            Action = new HashSet<string>(){"execute-api:Invoke"},
                            Effect = "Allow",
                            Resource = new HashSet<string>(){ "arn:aws:execute-api:*:*:*/*/*/sheets/*" }
                        },
                        new APIGatewayCustomAuthorizerPolicy.IAMPolicyStatement
                        {
                            Action = new HashSet<string>(){"execute-api:Invoke"},
                            Effect = effect,
                            Resource = new HashSet<string>(){ request.MethodArn }
                        }
                    },
                }
            };
        }
    }
}
