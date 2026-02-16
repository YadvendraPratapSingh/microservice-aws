using Microsoft.EntityFrameworkCore;

namespace CloudWeather.Temperature.DataAccess
{
    public class TemperatureDbContext : DbContext
    {
        public TemperatureDbContext()
        {

        }

        public TemperatureDbContext(DbContextOptions<TemperatureDbContext> opts) : base(opts)
        {

        }

        public DbSet<Temperature> Temperature { get; set; }

        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ConfigureTemperature(modelBuilder);
        }

        private static void ConfigureTemperature(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Temperature>(entity =>
            {
                entity.ToTable("temperature");
            });

        }
    }
}
