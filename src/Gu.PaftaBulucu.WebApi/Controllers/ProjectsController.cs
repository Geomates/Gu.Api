using System;
using System.Threading.Tasks;
using Gu.PaftaBulucu.Business.Dtos;
using Gu.PaftaBulucu.Business.Services;
using Gu.PaftaBulucu.WebApi.Extensions;
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
            var userEmail = HttpContext.User.GetEmail();
            var projects = await _projectService.GetProjects(userEmail);
            return Ok(projects);
        }

        [HttpPost]
        public async Task<IActionResult> AddProject([FromBody]SaveProjectDto projectDto)
        {
            var userEmail = HttpContext.User.GetEmail();
            if (string.IsNullOrEmpty(userEmail))
                return BadRequest("User e-mail is missing.");

            projectDto.Email = userEmail;

            var project = await _projectService.AddProject(projectDto);
            return Ok(project);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProject([FromBody]SaveProjectDto projectDto)
        {
            var userEmail = HttpContext.User.GetEmail();
            if (string.IsNullOrEmpty(userEmail))
                return BadRequest("User e-mail is missing.");

            projectDto.Email = userEmail;

            await _projectService.UpdateProject(projectDto);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var userEmail = HttpContext.User.GetEmail();
            if (string.IsNullOrEmpty(userEmail))
                return BadRequest("User e-mail is missing.");

            var project = await _projectService.GetProject(id);

            if (project == null)
                return NotFound();

            if (project.Email != userEmail)
                return Forbid();

            await _projectService.DeleteProject(id);

            return NoContent();
        }
    }
}
