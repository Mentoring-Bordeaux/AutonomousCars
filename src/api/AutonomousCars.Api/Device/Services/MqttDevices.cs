using AutonomousCars.Api.Models.Options;
using Microsoft.Extensions.Options;

namespace AutonomousCars.Api.Device.Services;

using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.EventGrid;

public class MqttDevices : IMqttDevices
{
    private readonly MqttConfiguration _mqttConfiguration;

    public MqttDevices(IOptions<MqttConfiguration> mqttConfiguration)
    {
        _mqttConfiguration = mqttConfiguration.Value;
    }
    public async Task<List<string>> GetDeviceNames()
    {
        TokenCredential cred = new DefaultAzureCredential();
        var client = new ArmClient(cred);

        var eventGridNamespaceResourceId = EventGridNamespaceResource.CreateResourceIdentifier(
            _mqttConfiguration.SubscriptionId, _mqttConfiguration.ResourceGroupName, _mqttConfiguration.EventGridNamespace);
        var eventGridNamespace = client.GetEventGridNamespaceResource(eventGridNamespaceResourceId);
        
        var collection = eventGridNamespace.GetEventGridNamespaceClients();
        
        var deviceNames = new List<string>();
        
        await foreach (var item in collection.GetAllAsync())
        {
            var resourceData = item.Data;
            resourceData.Attributes.TryGetValue("type", out var typeValue);
            if ("\"vehicle\"".Equals(typeValue?.ToString()))                
            {
                deviceNames.Add(resourceData.Name);
            }
        }

        return deviceNames;
    }
}
