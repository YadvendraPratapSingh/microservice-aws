using CloudWeather.DataLoader.Models;
using Microsoft.Extensions.Configuration;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var serviceConfig = config.GetSection("Services");

var tempServiceConfig = serviceConfig.GetSection("Temperature");
var tempServiceHost = tempServiceConfig.GetValue<string>("Host");
var tempServicePort = tempServiceConfig.GetValue<string>("Port");

var precipServiceConfig = serviceConfig.GetSection("Precipitation");
var precipServiceHost = precipServiceConfig.GetValue<string>("Host");
var precipServicePort = precipServiceConfig.GetValue<string>("Port");

var zipCodes = new List<string> { "98101", "98102", "98103", "98104", "98105" };

Console.WriteLine("Starting data load...");

var temperatureHttpClient = new HttpClient();
temperatureHttpClient.BaseAddress = new Uri($"http://{tempServiceHost}:{tempServicePort}");

var precipitationHttpClient = new HttpClient();
precipitationHttpClient.BaseAddress = new Uri($"http://{precipServiceHost}:{precipServicePort}");

foreach ( var code in zipCodes)
{
    Console.WriteLine($"Processing zip code {code}...");

    var from = DateTime.Now.AddYears(-2);
    var to = DateTime.Now;

    for (var date = from; date <= to; date = date.AddDays(1))
    {
        var temps = PostTemp(code, date, temperatureHttpClient);
        PostPrecip(temps[0], code, date, precipitationHttpClient);
        //var tempObservation = new
        //{
        //    ZipCode = code,
        //    TempHighF = Random.Shared.Next(50, 100),
        //    TempLowF = Random.Shared.Next(30, 49),
        //    CreatedOn = date
        //};
        //var precipObservation = new
        //{
        //    ZipCode = code,
        //    RainInches = Math.Round(Random.Shared.NextDouble() * 2, 2),
        //    SnowInches = Math.Round(Random.Shared.NextDouble() * 2, 2),
        //    CreatedOn = date
        //};
        //var tempResponse = await temperatureHttpClient.PostAsJsonAsync("/observation", tempObservation);
        //if (!tempResponse.IsSuccessStatusCode)
        //{
        //    Console.WriteLine($"Failed to post temperature observation for zip code {code} on {date.ToShortDateString()}. Status code: {tempResponse.StatusCode}");
        //}
        //var precipResponse = await precipitationHttpClient.PostAsJsonAsync("/observation", precipObservation);
        //if (!precipResponse.IsSuccessStatusCode)
        //{
        //    Console.WriteLine($"Failed to post precipitation observation for zip code {code} on {date.ToShortDateString()}. Status code: {precipResponse.StatusCode}");
        //}
    }
}

void PostPrecip(int lowTemp, object p, string zip, DateTime day, HttpClient precipitationHttpClient)
{
    var rand = new Random();
    var isPrecip = rand.Next(2) < 1;
    PrecipitationModel precipObservation;
    if (isPrecip)
    {
        var precipInches = Math.Round(rand.Next(1, 16));
        if(lowTemp < 32)
        {
            precipObservation = new PrecipitationModel
            {
                ZipCode = zip,
                AmountInches = precipInches,
                WeatherType = "Snow",
                CreatedOn = day
            };
        }
        else
        {
            precipObservation = new PrecipitationModel
            {
                ZipCode = zip,
                AmountInches = precipInches,
                WeatherType = "rain",
                CreatedOn = day
            };        
        }
    }
    else
    {
        precipObservation = new PrecipitationModel
        {
            ZipCode = zip,
            AmountInches = 0,
            WeatherType = "None",
            CreatedOn = day
        };
    }

    var precipResponse = precipitationHttpClient.PostAsJsonAsync("/observation", precipObservation).Result;
    if (precipResponse.IsSuccessStatusCode)
    {
        Console.WriteLine($"posted precipitation: Date: {day:d} " +
                           $"zip code: {zip} " +
                           $"Type: {precipObservation.WeatherType} " +
                           $"Amount: {precipObservation.AmountInches} ");
    }
}

List<int> PostTemp(string zip, DateTime day, HttpClient temperatureHttpClient)
{
   var rand = new Random();
    var tempHigh = rand.Next(0, 100);
    var tempLow = rand.Next(0, 20);

    var highLow = new List<int> { tempHigh, tempLow };
    highLow.Sort();

    var tempObservation = new TemperatureModel
    {
        ZipCode = zip,
        TempHighF = tempHigh,
        TempLowF = tempLow,
        CreatedOn = day
    };

    var tempResponse = temperatureHttpClient.PostAsJsonAsync("/observation", tempObservation).Result;
    return highLow;

}


