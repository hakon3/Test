using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Second.Contracts;
using Second.Data;

namespace Second.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityWeatherController : ControllerBase
    {
        private readonly SecondDbContext _context;

        public CityWeatherController(SecondDbContext context)
        {
            _context = context;
        }

        [HttpGet("minTempByMinute")]
        public async Task<ActionResult<IEnumerable<CityWeatherContract>>> MinTemp()
        {
            var cityWeatherByMinute = await GetCityWeatherByMinute();
            var minimumCityTempByMinute = cityWeatherByMinute.Select(x =>
                (new DateTime(x.Key * TimeSpan.TicksPerMinute), x.OrderBy(y => y.Temperature).First()));

            var cityWeatherDtos = MapCityWeatherContracts(minimumCityTempByMinute);

            return cityWeatherDtos;
        }

        [HttpGet("maxWindSpeedByMinute")]
        public async Task<ActionResult<IEnumerable<CityWeatherContract>>> MaxWindSpeed()
        {
            var cityWeatherByMinute = await GetCityWeatherByMinute();
            var maximumCityWindSpeedByMinute
                = cityWeatherByMinute.Select(x =>
                    (new DateTime(x.Key * TimeSpan.TicksPerMinute), x.OrderByDescending(y => y.WindSpeed).First()));

            var cityWeatherDtos = MapCityWeatherContracts(maximumCityWindSpeedByMinute);

            return cityWeatherDtos;
        }

        [HttpGet("weatherTrendLastTwoHours")]
        public async Task<ActionResult<WeatherTrendContract>> WeatherTrend()
        {
            var cityWeatherByMinute = await GetCityWeatherByMinute(twoHoursCutoff: true);
            if(cityWeatherByMinute.Count() < 2)
            { 
                return BadRequest("Not enough data to calculate weather trend");
            }

            var firstTemp = cityWeatherByMinute.First().Average(i => i.Temperature);
            var lastTemp = cityWeatherByMinute.Last().Average(i => i.Temperature);

            var firstWindSpeed = cityWeatherByMinute.First().Average(i => i.WindSpeed);
            var lastWindSpeed = cityWeatherByMinute.Last().Average(i => i.WindSpeed);

            string temperatureTrend;
            if (firstTemp < lastTemp)
            {
                temperatureTrend = "increasing";
            }
            else if (firstTemp > lastTemp)
            {
                temperatureTrend = "decreasing";
            }
            else
            {
                temperatureTrend = "constant";
            }

            string windSpeedTrend;
            if (firstWindSpeed < lastWindSpeed)
            {
                windSpeedTrend = "increasing";
            }
            else if (firstWindSpeed > lastWindSpeed)
            {
                windSpeedTrend = "decreasing";
            }
            else
            {
                windSpeedTrend = "constant";
            }

            var result = new WeatherTrendContract
            {
                TemperatureTrend = temperatureTrend,
                WindTrend = windSpeedTrend
            };

            return result;
        }

        private async Task<IEnumerable<IGrouping<long, CityWeather>>> GetCityWeatherByMinute(bool twoHoursCutoff = false)
        {
            List<CityWeather> cityWeather;
            if (twoHoursCutoff)
            {
                cityWeather = await _context.CityWeather.Where(i => i.Time >= DateTimeOffset.Now.AddHours(-4))
                    .Include(x => x.CityNavigation)
                    .ToListAsync();
            }
            else
            {
                cityWeather = await _context.CityWeather
                    .Include(x => x.CityNavigation)
                    .ToListAsync();
            }

            var cityWeathersByMinute =
                cityWeather.GroupBy(x => x.Time.Ticks / TimeSpan.TicksPerMinute).OrderBy(i => i.Key);
            return cityWeathersByMinute;
        }

        private static List<CityWeatherContract> MapCityWeatherContracts(IEnumerable<(DateTime, CityWeather)> minimumCityTempByMinute)
        {
            var cityWeatherDtos = new List<CityWeatherContract>();
            foreach (var cityWeatherItem in minimumCityTempByMinute)
            {
                var cityWeatherDto = new CityWeatherContract
                {
                    Time = cityWeatherItem.Item1,
                    Temperature = cityWeatherItem.Item2.Temperature,
                    WindSpeed = cityWeatherItem.Item2.WindSpeed,
                    CityName = cityWeatherItem.Item2.CityNavigation.Name,
                    Country = cityWeatherItem.Item2.CityNavigation.Country
                };
                cityWeatherDtos.Add(cityWeatherDto);
            }

            return cityWeatherDtos;
        }
    }
}
