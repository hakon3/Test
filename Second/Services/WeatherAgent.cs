using Second.Data;
using Second.Gateway;

namespace Second.Services;

public class WeatherAgent : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public WeatherAgent(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var secondDbContext = scope.ServiceProvider.GetRequiredService<SecondDbContext>();
        var weatherGateway = scope.ServiceProvider.GetRequiredService<IWeatherGateway>();
        while (!stoppingToken.IsCancellationRequested)
        {
            var citiesToFetchWeatherFor = secondDbContext.Cities.ToList();
            foreach (var city in citiesToFetchWeatherFor)
            {
                var weather = await weatherGateway.GetCurrentWeather(city.Latitude, city.Longitude);
                if (weather != null)
                {
                    var cityWeather = new CityWeather
                    {
                        CityNavigation = city,
                        Temperature = weather.Temperature,
                        WindSpeed = weather.WindSpeed,
                        Cloudiness = weather.Clouds ? Cloudiness.Cloudy : Cloudiness.Clear,
                        Time = weather.Time
                    };
                    secondDbContext.CityWeather.Add(cityWeather);
                    await secondDbContext.SaveChangesAsync(CancellationToken.None);

                    // can potentially wait also here to not overload the weather service
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    }
}