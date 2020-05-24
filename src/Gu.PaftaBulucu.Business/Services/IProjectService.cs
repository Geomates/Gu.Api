using System.Collections.Generic;
using System.Threading.Tasks;
using Gu.PaftaBulucu.Business.Dtos;

namespace Gu.PaftaBulucu.Business.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<ListProjectDto>> GetProjects(string email);
        Task<int> SaveProject(SaveProjectDto projectDto);
        Task SaveProject(int projectId, SaveProjectDto projectDto);
    }
}