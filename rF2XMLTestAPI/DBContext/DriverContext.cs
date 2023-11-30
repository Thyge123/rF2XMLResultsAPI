using rF2XMLTestAPI.Model;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace rF2XMLTestAPI.DBContext
{
    public class DriverContext : DbContext
    {
        public DriverContext(DbContextOptions<DriverContext> options) : base(options)
        {
        }

        public DbSet<Driver> Drivers { get; set; }
    }
}
