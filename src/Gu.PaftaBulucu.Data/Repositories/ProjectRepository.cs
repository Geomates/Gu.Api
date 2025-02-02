using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Gu.PaftaBulucu.Data.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gu.PaftaBulucu.Data.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DynamoDBContext _dynamoDbContext;

        public ProjectRepository(IAmazonDynamoDB dynamoDB, IConfiguration configuration)
        {
            _dynamoDbContext = new DynamoDBContext(dynamoDB);
        }

        public async Task UpsertAsync(Project entity)
        {
            await _dynamoDbContext.SaveAsync(entity);
        }

        public async Task<IEnumerable<Project>> FindByEmailAsync(string email)
        {
            var queryConfig = new QueryOperationConfig
            {
                IndexName = "EmailIndex",
                KeyExpression = new Expression
                {
                    ExpressionStatement = "Email = :email",
                    ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>
                    {
                        { ":email", email }
                    }
                }
            };

            var queryResult = await _dynamoDbContext.FromQueryAsync<Project>(queryConfig).GetRemainingAsync();
            return queryResult;
        }

        public async ValueTask<Project> GetByIdAsync(int id)
        {
            var retrievedEntity = await _dynamoDbContext.LoadAsync<Project>(id);
            return retrievedEntity;
        }

        public async Task RemoveAsync(Project entity)
        {
            await _dynamoDbContext.DeleteAsync<Project>(entity.ProjectId);
        }
    }
}
