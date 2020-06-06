using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using System.Threading.Tasks;

namespace Gu.PaftaBulucu.Business.Services
{
    public class ParameterService : IParameterService
    {
        private readonly IAmazonSimpleSystemsManagement _amazonSimpleSystemsManagement;

        public ParameterService(IAmazonSimpleSystemsManagement amazonSimpleSystemsManagement)
        {
            _amazonSimpleSystemsManagement = amazonSimpleSystemsManagement;
        }

        public async Task<string> GetParameterValueAsync(string parameterName)
        {
            var response = await _amazonSimpleSystemsManagement.GetParameterAsync(new GetParameterRequest
            {
                Name = parameterName
            });

            return response?.Parameter?.Value;
        }
    }
}
