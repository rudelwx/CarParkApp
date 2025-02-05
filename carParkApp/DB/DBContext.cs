using carParkApp.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace carParkApp.DB
{
    public class DBContext: DbContext
    {
        public DBContext(DbContextOptions<DBContext> options):base(options) { }

        public DbSet<car_park_hdr> car_park_hdr { get; set; }
        public DbSet<car_park_dtl> car_park_dtl { get; set; }
    }
}
