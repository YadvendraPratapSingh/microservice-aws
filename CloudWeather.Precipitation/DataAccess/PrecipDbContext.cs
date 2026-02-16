using Microsoft.EntityFrameworkCore;

namespace CloudWeather.Precipitation.DataAccess
{
    public class PrecipDbContext : DbContext
    {
        public PrecipDbContext()
        {
            
        }

        public PrecipDbContext(DbContextOptions<PrecipDbContext> options): base(options)
        {
            
        }

        public DbSet<Precipitation> Precipitations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ConfigurePrecipitation(modelBuilder);
        }

        private static void ConfigurePrecipitation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Precipitation>(entity =>
            {
                entity.ToTable("precipitations");              
            });
        }
    }
}
