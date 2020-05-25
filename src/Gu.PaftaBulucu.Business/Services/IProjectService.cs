using System.Collections.Generic;
using System.Threading.Tasks;
using Gu.PaftaBulucu.Business.Dtos;

namespace Gu.PaftaBulucu.Business.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetProjects(string email);
        Task<int> AddProject(string email, SaveProjectDto projectDto);
        Task UpdateProject(ProjectDto projectDto);
        Task<ProjectDto> GetProject(int projectId);
        Task DeleteProject(int projectId);
    }
}