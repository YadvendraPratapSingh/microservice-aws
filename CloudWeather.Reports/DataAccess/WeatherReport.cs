namespace CloudWeather.Reports.DataAccess
{
    public class WeatherReport
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public Decimal AverageHighF { get; set; }
        public Decimal AverageLowF { get; set; }

        public Decimal RainFallTotalInches { get; set; }

        public Decimal SnowTotalInches { get; set; }

        public string ZipCode { get; set; }
    }
}
