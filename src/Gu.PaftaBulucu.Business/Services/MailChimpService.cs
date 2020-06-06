using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Gu.PaftaBulucu.Business.Services
{
    public class MailChimpService : IMailChimpService
    {
        private readonly ILogger<MailChimpService> _logger;
        private readonly string _apiKey;
        private readonly string _listId;
        private readonly string _baseUrl;

        public MailChimpService(IConfiguration configuration, ILogger<MailChimpService> logger)
        {
            _logger = logger;
            _apiKey = configuration["MailChimp:ApiKey"];
            _listId = configuration["MailChimp:ListId"];
            _baseUrl = configuration["MailChimp:BaseUrl"];
        }

        public async Task<bool> AddMemberAsync(string email)
        {
            using (var httpClient = new HttpClient())
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/lists/{_listId}/members");
                var byteArray = Encoding.ASCII.GetBytes($"apikey:{_apiKey}");
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                requestMessage.Content = new StringContent("{\"email_address\":\"" + email + "\",\"status\":\"subscribed\"}");

                var response =  await httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var json = JObject.Parse(responseBody);
                    if (json.ContainsKey("title") && json["title"].Value<string>() == "Member Exists")
                        return true;
                }
                _logger.LogError($"User cannot subscribed - API Response Code: {response.StatusCode} Response Body: {responseBody}");
            }
            return false;
        }
    }
}
