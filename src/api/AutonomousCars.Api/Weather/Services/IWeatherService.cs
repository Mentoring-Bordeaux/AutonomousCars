namespace AutonomousCars.Api.Weather.Services;

public interface IWeatherService
{
    WeatherForecast[] GetForecast();
}
