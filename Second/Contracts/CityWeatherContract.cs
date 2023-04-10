using Second.Data;

namespace Second.Contracts;

public record CityWeatherContract
{
    public DateTimeOffset Time { get; set; }
    public double Temperature { get; set; }
    public double WindSpeed { get; set; }
    public string CityName { get; set; }
    public string Country { get; set; }
}