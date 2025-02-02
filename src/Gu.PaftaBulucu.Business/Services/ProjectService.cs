using Gu.PaftaBulucu.Business.Dtos;
using Gu.PaftaBulucu.Data.Models;
using Gu.PaftaBulucu.Data.Repositories;
using Nelibur.ObjectMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gu.PaftaBulucu.Business.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<ProjectDto>> GetProjects(string email)
        {
            TinyMapper.Bind<List<Project>, List<ProjectDto>>();
            var projects = await _projectRepository.FindByEmailAsync(email);
            return TinyMapper.Map<List<ProjectDto>>(projects);
        }

        public async Task<ProjectDto> GetProject(int projectId)
        {
            TinyMapper.Bind<Project, ProjectDto>();
            var project = await _projectRepository.GetByIdAsync(projectId);
            return TinyMapper.Map<ProjectDto>(project);
        }

        public async Task<ProjectDto> AddProject(SaveProjectDto projectDto)
        {
            TinyMapper.Bind<SaveProjectDto, Project>();
            TinyMapper.Bind<Project, ProjectDto>();
            var project = TinyMapper.Map<Project>(projectDto);

            project.ProjectId = UnixTimeStamp(); //TODO: This should be UUID
            project.Created = UnixTimeStamp();

            await _projectRepository.UpsertAsync(project);

            return TinyMapper.Map<ProjectDto>(project);
        }

        public async Task UpdateProject(SaveProjectDto projectDto)
        {
            TinyMapper.Bind<List<SheetEntryDto>, List<SheetEntry>>();

            var project = await _projectRepository.GetByIdAsync(projectDto.ProjectId);
            
            if (project == null)
                throw new ArgumentException("Project not found");

            if (project.Email != projectDto.Email)
                throw new ArgumentException("Project e-mail is not matched with user e-mail");

            project.Entries = TinyMapper.Map<List<SheetEntry>>(projectDto.Entries);
            project.Name = projectDto.Name;

            await _projectRepository.UpsertAsync(project);
        }

        public async Task DeleteProject(int projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);

            if (project == null)
                return;

            await _projectRepository.RemoveAsync(project);
        }

        private int UnixTimeStamp()
        {
            return (int)DateTime.UtcNow.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }
    }
}
