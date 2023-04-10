namespace Second.Services;

public class CityWeatherDto
{
    public double Temperature { get; set; }
    public double WindSpeed { get; set; }
    public bool Clouds { get; set; }
    public DateTimeOffset Time { get; set; } 
}