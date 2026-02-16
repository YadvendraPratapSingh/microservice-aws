using Microsoft.EntityFrameworkCore;

namespace CloudWeather.Reports.DataAccess
{
    public class WeatherReportDbContext : DbContext
    {
        public WeatherReportDbContext()
        {

        }

        public WeatherReportDbContext(DbContextOptions<WeatherReportDbContext> opts) : base(opts)
        {

        }

        public DbSet<WeatherReport> WeatherReport { get; set; }

        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ConfigureWeatherReport(modelBuilder);
        }

        private static void ConfigureWeatherReport(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WeatherReport>(entity =>
            {
                entity.ToTable("weatherReport");
            });

        }
    }
}
