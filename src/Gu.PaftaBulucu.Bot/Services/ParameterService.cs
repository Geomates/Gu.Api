using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using System.Threading.Tasks;

namespace Gu.PaftaBulucu.Bot.Services
{
    public interface IParameterService
    {
        Task<string> GetTelegramToken();
    }

    public class ParameterService : IParameterService
    {
        private readonly IAmazonSimpleSystemsManagement _amazonSimpleSystemsManagement;
        private string _telegramToken;

        public ParameterService(IAmazonSimpleSystemsManagement amazonSimpleSystemsManagement)
        {
            _amazonSimpleSystemsManagement = amazonSimpleSystemsManagement;
        }

        public async Task<string> GetTelegramToken()
        {
            if (!string.IsNullOrEmpty(_telegramToken))
                return _telegramToken;

            var response = await _amazonSimpleSystemsManagement.GetParameterAsync(new GetParameterRequest
            {
                Name = "/gu/bot/telegram/token"
            });

            _telegramToken = response?.Parameter?.Value;

            return _telegramToken;
        }
    }
}
