using AutonomousCars.Api.Models.Options;

namespace AutonomousCars.Api.Device.Services;

using System;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.EventGrid;
public class MqttDevices : IMqttDevices
{
    public async Task<List<String>> GetDeviceNames(MqttNamespaceOptions mqttNamespaceOptions)
    {
        TokenCredential cred = new DefaultAzureCredential();
        ArmClient client = new ArmClient(cred);

        
        ResourceIdentifier eventGridNamespaceResourceId = EventGridNamespaceResource.CreateResourceIdentifier(mqttNamespaceOptions.SubscriptionId, mqttNamespaceOptions.ResourceGroupName, mqttNamespaceOptions.Namespace);
        EventGridNamespaceResource eventGridNamespace = client.GetEventGridNamespaceResource(eventGridNamespaceResourceId);
        
        EventGridNamespaceClientCollection collection = eventGridNamespace.GetEventGridNamespaceClients();
        
        var deviceNames = new List<string>();
        
        await foreach (EventGridNamespaceClientResource item in collection.GetAllAsync())
        {
            EventGridNamespaceClientData resourceData = item.Data;
            resourceData.Attributes.TryGetValue("type", out var typeValue);
            if ("\"vehicle\"".Equals(typeValue?.ToString()))
            {
                deviceNames.Add(resourceData.Name);
            }
        }

        return deviceNames;
    }
}
