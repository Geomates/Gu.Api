using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gu.PaftaBulucu.Bot.Services
{
    public interface IAmazonDynamoDbService
    {
        Task<bool> UpdateAsync(int chatId, double lat, double lon);
        Task<(double lat, double lon)> QueryAsync(int chatId);
    }

    public class AmazonDynamoDbService : IAmazonDynamoDbService
    {
        private const string TABLE_NAME = "DYNAMODB_TABLE_NAME";

        private readonly IAmazonDynamoDB _amazonDynamoDb;
        private readonly string _tableName;

        public AmazonDynamoDbService(IAmazonDynamoDB amazonDynamoDb)
        {
            _amazonDynamoDb = amazonDynamoDb;
            _tableName = Environment.GetEnvironmentVariable(TABLE_NAME);
        }

        public async Task<bool> UpdateAsync(int chatId, double lat, double lon)
        {
            var updateItemRequest = new PutItemRequest()
            {
                TableName = _tableName,
                Item = new Dictionary<string, AttributeValue>
                {
                    { "chatid", new AttributeValue { N = chatId.ToString() } },
                    { "Latitude", new AttributeValue { N = lat.ToString() } },
                    { "Longitude", new AttributeValue { N = lon.ToString() } }
                }
            };

            var response = await _amazonDynamoDb.PutItemAsync(updateItemRequest);

            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public async Task<(double lat, double lon)> QueryAsync(int chatId)
        {
            var queryRequest = new GetItemRequest(_tableName, new Dictionary<string, AttributeValue>
            {
                { "chatid",   new AttributeValue { N = chatId.ToString() } }
            });

            var response = await _amazonDynamoDb.GetItemAsync(queryRequest);
            (double lat, double lon) result = (0,0);

            if (response.HttpStatusCode != System.Net.HttpStatusCode.OK) 
                return result;
            
            result.lat = double.Parse(response.Item["Latitude"].N);
            result.lon = double.Parse(response.Item["Longitude"].N);

            return result;
        }
    }
}
