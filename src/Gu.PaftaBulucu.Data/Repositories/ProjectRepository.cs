using Gu.PaftaBulucu.Data.Models;

namespace Gu.PaftaBulucu.Data.Repositories
{
    public class ProjectRepository : DatabaseRepositoryBase<Project>
    {
        public ProjectRepository(GuDbContext guDbContext) : base(guDbContext) {}
    }
}
