using Microsoft.EntityFrameworkCore;
using rF2XMLTestAPI.Model;

namespace rF2XMLTestAPI.DBContext
{
    public class TrackContext : DbContext
    {
        public TrackContext(DbContextOptions<TrackContext> options) : base(options)
        {

        }
        public DbSet<Track> Tracks { get; set; }
    }
}
