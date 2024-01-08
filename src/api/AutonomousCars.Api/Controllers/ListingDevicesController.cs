namespace AutonomousCars.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using AutonomousCars.Api.Device.Services;

[ApiController]
[Route("api/[controller]")]
public class ListingDevicesController : Controller
{
    private readonly IMqttDevices _mqttDevices;

    public ListingDevicesController(IMqttDevices mqttDevices)
    {
        _mqttDevices = mqttDevices;
    }
    
    [HttpGet("getAllDevices")]
    public async Task<IActionResult> GetAllDevices()
    {
        var response = new
        {
            Status = "Success",
            DeviceNames = await _mqttDevices.GetDeviceNames()
        };
        return Ok(response);
    }
}