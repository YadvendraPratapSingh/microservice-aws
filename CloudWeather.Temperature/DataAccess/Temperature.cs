namespace CloudWeather.Temperature.DataAccess
{
    public class Temperature
    {
        public Guid Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public Decimal TempHighF { get; set; }
        public Decimal TempLowF { get; set; }

        public string ZipCode { get; set; }
    }
}
