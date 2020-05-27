using Gu.PaftaBulucu.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Gu.PaftaBulucu.Data
{
    public class GuDbContext : DbContext
    {
        public GuDbContext(DbContextOptions<GuDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
    }
}
