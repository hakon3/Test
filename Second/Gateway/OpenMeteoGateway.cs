using System.Text.Json.Nodes;
using Second.Services;

namespace Second.Gateway
{
    public interface IWeatherGateway
    {
        Task<CityWeatherDto> GetCurrentWeather(string lat, string lon);
    }

    public class OpenMeteoGateway : IWeatherGateway
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OpenMeteoGateway(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<CityWeatherDto> GetCurrentWeather(string lat, string lon)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var result = await httpClient.GetAsync($"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current_weather=true");
            if (result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                var weatherJson = JsonNode.Parse(content);
                var currentWeather = weatherJson["current_weather"];
                var weatherNode = new CityWeatherDto
                {
                    Temperature = currentWeather["temperature"].GetValue<double>(),
                    WindSpeed = currentWeather["windspeed"].GetValue<double>(),
                    Clouds = GetClouds(currentWeather["weathercode"].GetValue<int>()),
                    Time = currentWeather["time"].GetValue<DateTimeOffset>()
                };
                return weatherNode;
            }

            return null;
        }

        private static bool GetClouds(int weatherCode)
        {
            return weatherCode > 1;
        }
    }
}
