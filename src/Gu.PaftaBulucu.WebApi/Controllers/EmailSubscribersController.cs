using System.Net;
using System.Threading.Tasks;
using Gu.PaftaBulucu.Business.Dtos;
using Gu.PaftaBulucu.Business.Services;
using Gu.PaftaBulucu.Business.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Gu.PaftaBulucu.WebApi.Controllers
{
    [Route("[controller]")]
    public class EmailSubscribersController : Controller
    {
        private readonly IMailChimpService _mailChimpService;

        public EmailSubscribersController(IMailChimpService mailChimpService)
        {
            _mailChimpService = mailChimpService;

        }

        [HttpPost]
        public async Task<IActionResult> AddSubscriber([FromBody]AddSubscriberDto addSubscriberDto)
        {
            if (!RegexUtilities.IsValidEmail(addSubscriberDto.Email))
            {
                return BadRequest("E-posta adresi yanlış veya eksik!");
            }

            var response = await _mailChimpService.AddMemberAsync(addSubscriberDto.Email);
            if (response)
            {
                return Ok();
            }

            return Problem(statusCode: (int)HttpStatusCode.InternalServerError);
        }
    }
}
