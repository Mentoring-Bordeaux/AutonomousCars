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
        string subscriptionId = _mqttNamespaceOptions.SubscriptionId ?? throw new MissingSettingException($"{nameof(MqttNamespaceOptions)}.{nameof(MqttNamespaceOptions.SubscriptionId)}");
        string resourceGroupName = _mqttNamespaceOptions.ResourceGroupName ?? throw new MissingSettingException($"{nameof(MqttNamespaceOptions)}.{nameof(MqttNamespaceOptions.ResourceGroupName)}");
        string namespaceName =  _mqttNamespaceOptions.NamespaceName ?? throw new MissingSettingException($"{nameof(MqttNamespaceOptions)}.{nameof(MqttNamespaceOptions.NamespaceName)}");

        var response = new
        {
            Status = "Success",
            DeviceNames = await _mqttDevices.GetDeviceNames(subscriptionId, resourceGroupName, namespaceName)
        };
        return Ok(response);
    }
}