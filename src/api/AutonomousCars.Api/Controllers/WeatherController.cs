namespace AutonomousCars.Api.Controllers;

using Weather.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/weatherforecast")]
public class WeatherController : Controller
{
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet]
    public IActionResult GetWeather()
    {
        return Ok(_weatherService.GetForecast());
    }
}
