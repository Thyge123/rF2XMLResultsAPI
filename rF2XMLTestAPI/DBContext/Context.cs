using Microsoft.EntityFrameworkCore;
using rF2XMLTestAPI.Model;

namespace rF2XMLTestAPI.DBContext
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Lap> Laps { get; set; }
        public DbSet<RaceResults> RaceResults { get; set; }
    }
}
