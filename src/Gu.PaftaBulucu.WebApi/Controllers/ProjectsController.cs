using System;
using System.Threading.Tasks;
using Gu.PaftaBulucu.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gu.PaftaBulucu.WebApi.Controllers
{
    [Route("[controller]")]
    public class ProjectsController : Controller
    {
        private readonly IProjectService _projectService;
        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            foreach (var userClaim in HttpContext.User.Claims)
            {
                Console.WriteLine($"{userClaim.Type} : ${userClaim.Value}");
            }

            var userEmail = HttpContext.User.FindFirst("principalId")?.Value;
            var projects = await _projectService.GetProjects(userEmail);
            return Ok(projects);
        }
    }
}
