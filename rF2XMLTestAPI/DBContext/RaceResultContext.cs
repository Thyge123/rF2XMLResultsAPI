using Microsoft.EntityFrameworkCore;
using rF2XMLTestAPI.Model;

namespace rF2XMLTestAPI.DBContext
{
    public class RaceResultContext : DbContext
    {
        public RaceResultContext(DbContextOptions<RaceResultContext> options) : base(options)
        {
        }

        public DbSet<RaceResults> RaceResults { get; set; }
    }
}
