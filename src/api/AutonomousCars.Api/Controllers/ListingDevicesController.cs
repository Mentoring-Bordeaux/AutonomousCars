using AutonomousCars.Api.Models.Options;

namespace AutonomousCars.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

using Device.Services;


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
        var mqttNamespaceOptions = MqttSettings.MqttNamespaceOptions;

        if (mqttNamespaceOptions != null)
        {
            var response = new
            {
                Status = "Success",
                DeviceNames = await _mqttDevices.GetDeviceNames(mqttNamespaceOptions)
            };
            return Ok(response);
        }
        else
        {
            var errorResponse = new
            {
                Status = "Error",
                Message = "Issue of configuration.",
            };
            return BadRequest(errorResponse);
        }

    }
}