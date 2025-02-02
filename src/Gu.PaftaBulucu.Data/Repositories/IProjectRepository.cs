using Gu.PaftaBulucu.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gu.PaftaBulucu.Data.Repositories
{
    public interface IProjectRepository
    {
        ValueTask<Project> GetByIdAsync(int id);
        Task<IEnumerable<Project>> FindByEmailAsync(string email);
        Task UpsertAsync(Project entity);
        Task RemoveAsync(Project entity);
    }
}
