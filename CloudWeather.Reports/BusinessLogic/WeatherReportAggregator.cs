using CloudWeather.Reports.Config;
using CloudWeather.Reports.DataAccess;
using CloudWeather.Reports.Models;
using Microsoft.Extensions.Options;

namespace CloudWeather.Reports.BusinessLogic
{
    public interface IWeatherReportAggregator
    {
        public Task<WeatherReport> BuildReport(string zipCode, int days);
    }

    public class WeatherReportAggregator : IWeatherReportAggregator
    {      

        public WeatherReportAggregator(IHttpClientFactory http, ILogger<WeatherReportAggregator> logger, IOptions<WeatherDataConfig> weatherConfig, WeatherReportDbContext db)
        {
            Http = http;
            Logger = logger;
            WeatherConfig = weatherConfig;
            Db = db;
        }

        public IHttpClientFactory Http { get; }
        public ILogger<WeatherReportAggregator> Logger { get; }
        public IOptions<WeatherDataConfig> WeatherConfig { get; }
        public WeatherReportDbContext Db { get; }

        public async Task<WeatherReport> BuildReport(string zipCode, int days)
        {
            var httpClient = Http.CreateClient();
            var precipData = await FetchPrecipitationData(httpClient, zipCode, days);
            var totalSnow = GetTotalSnow(precipData);
            var totalRain = GetTotalRain(precipData);
            Logger.LogInformation("Total Snow: {TotalSnow} inches, Total Rain: {TotalRain} inches for zip code {ZipCode} over the past {Days} days.", totalSnow, totalRain, zipCode, days);
            var tempData = await FetchTemperatureData(httpClient, zipCode, days);
            var averageHigh = tempData.Average(t => t.TempHighF);
            var averageLow = tempData.Average(t => t.TempLowF);
            Logger.LogInformation("Average High: {AverageHigh} F, Average Low: {AverageLow} F for zip code {ZipCode} over the past {Days} days.", averageHigh, averageLow, zipCode, days);

            var weatherReport = new WeatherReport
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                AverageHighF = Math.Round(averageHigh, 1),
                AverageLowF = Math.Round(averageLow, 1),
                RainFallTotalInches = totalRain,
                SnowTotalInches = totalSnow,
                ZipCode = zipCode
            };

            // TODO: Use 'cached' weather reports instead of making round trip when possible. This will require some thought around cache invalidation and report freshness, but will be critical for scaling the service.
            Db.Add(weatherReport);
            await Db.SaveChangesAsync();

            return weatherReport;
        }

        private async Task<List<TemperatureModel>> FetchTemperatureData(HttpClient httpClient, string zipCode, int days)
        {
            var endPoint = BuildTemperatureServiceEndPoint(zipCode, days);
            var temperatureRecords = await httpClient.GetAsync(endPoint);
            var jsonSerializerOptions = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            };
            var temperatureData = await temperatureRecords.Content.ReadFromJsonAsync<List<TemperatureModel>>(jsonSerializerOptions);
            return temperatureData ?? new List<TemperatureModel>();
        }

        private string? BuildTemperatureServiceEndPoint(string zipCode, int days)
        {
           var tempServiceProtocol = WeatherConfig.Value.TempDataProtocol;
            Console.WriteLine(tempServiceProtocol);
            var tempServiceHost = WeatherConfig.Value.TempDataHost;
            var tempServicePort = WeatherConfig.Value.TempDataPort;
           // Console.WriteLine("Temperature service endpoint: {TempServiceProtocol}://{TempServiceHost}:{TempServicePort}", tempServiceProtocol, tempServiceHost, tempServicePort);
            return $"{tempServiceProtocol}://{tempServiceHost}:{tempServicePort}/observation/{zipCode}?days={days}";
        }

        private async Task<List<PrecipitationModel>> FetchPrecipitationData(HttpClient httpClient, string zipCode, int days)
        {
            var endPoint = BuildPrecipitationServiceEndPoint(zipCode, days);
            var precipRecords = await httpClient.GetAsync(endPoint);
            var jsonSerializerOptions = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            };

            var precipData = await precipRecords.Content.ReadFromJsonAsync<List<PrecipitationModel>>(jsonSerializerOptions);
            return precipData ?? new List<PrecipitationModel>();
        }

        private string BuildPrecipitationServiceEndPoint(string zipCode, int days)
        {
            try
            {
                Console.WriteLine("checking....");
                var precipServiceProtocol = WeatherConfig.Value.PrecipDataProtocol;
                Console.WriteLine(precipServiceProtocol);
                var precipServiceHost = WeatherConfig.Value.PrecipDataHost;
                var precipServicePort = WeatherConfig.Value.PrecipDataPort;
                Console.WriteLine($"Precipitation service endpoint: {precipServiceProtocol}://{precipServiceHost}:{precipServicePort}");
                return $"{precipServiceProtocol}://{precipServiceHost}:{precipServicePort}/observation/{zipCode}?days={days}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error building precipitation service endpoint: { ex.Message}");
                throw;
            }

        }

        private static decimal GetTotalSnow(IEnumerable<PrecipitationModel> precipData)
        {             
            return Math.Round(precipData.Where(p => p.WeatherType == "snow").Sum(p => p.AmountInches), 1);
        }

        private static decimal GetTotalRain(IEnumerable<PrecipitationModel> precipData)
        {
            return Math.Round(precipData.Where(p => p.WeatherType == "rain").Sum(p => p.AmountInches), 1);
        }
    }
}
