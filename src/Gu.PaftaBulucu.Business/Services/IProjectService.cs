using System.Collections.Generic;
using System.Threading.Tasks;
using Gu.PaftaBulucu.Business.Dtos;

namespace Gu.PaftaBulucu.Business.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetProjects(string email);
        Task<ProjectDto> AddProject(SaveProjectDto projectDto);
        Task UpdateProject(SaveProjectDto projectDto);
        Task<ProjectDto> GetProject(int projectId);
        Task DeleteProject(int projectId);
    }
}