using AutonomousCars.Api.Models.Options;

namespace AutonomousCars.Api.Controllers;
using AutonomousCars.Api.Models.Exceptions;

using Microsoft.AspNetCore.Mvc;

using AutonomousCars.Api.Device.Services;
using Microsoft.Extensions.Options;


[ApiController]
[Route("api/[controller]")]
public class ListingDevicesController : Controller
{
    private readonly IMqttDevices _mqttDevices;
    private readonly MqttNamespaceOptions _mqttNamespaceOptions;
    
    public ListingDevicesController(IMqttDevices mqttDevices, IOptions<MqttNamespaceOptions> mqttNamespaceOptions)
    {
        _mqttDevices = mqttDevices;
        _mqttNamespaceOptions = mqttNamespaceOptions.Value;
    }
    
    [HttpGet("getAllDevices")]
    public async Task<IActionResult> GetAllDevices()
    {
        MqttNamespaceOptions? mqttNamespaceOptions = MqttSettings.MqttNamespaceOptions;

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