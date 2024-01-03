using Microsoft.EntityFrameworkCore;
using rF2XMLTestAPI.Model;

namespace rF2XMLTestAPI.DBContext
{
    public class LapsContext : DbContext
    {
        public LapsContext(DbContextOptions<LapsContext> options) : base(options)
        {
        }

        public LapsContext()
        {
            
        }

        public DbSet<Lap> Laps { get; set; }
    }
}
