using System;
using System.Threading.Tasks;
using Gu.PaftaBulucu.Business.Dtos;
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
            var userEmail = HttpContext.User.FindFirst("principalId")?.Value;
            var projects = await _projectService.GetProjects(userEmail);
            return Ok(projects);
        }

        [HttpPost]
        public async Task<IActionResult> AddProject([FromBody]SaveProjectDto projectDto)
        {
            var userEmail = HttpContext.User.FindFirst("principalId")?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return BadRequest("User e-mail is missing.");

            var projectId = await _projectService.AddProject(userEmail, projectDto);
            return Ok(projectId);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody]SaveProjectDto projectDto)
        {
            var userEmail = HttpContext.User.FindFirst("principalId")?.Value;
            if (string.IsNullOrEmpty(userEmail))
                return BadRequest("User e-mail is missing.");

            var project = await _projectService.GetProject(id);

            if (project == null)
                return NotFound();

            if (project.Email != userEmail)
                return Forbid();

            project.Entries = projectDto.Entries;
            project.Name = projectDto.Name;

            await _projectService.UpdateProject(project);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var userEmail = HttpContext.User.FindFirst("principalId")?.Value;
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
