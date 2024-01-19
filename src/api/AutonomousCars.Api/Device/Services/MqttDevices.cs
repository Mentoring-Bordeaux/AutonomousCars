using AutonomousCars.Api.Models.Options;

namespace AutonomousCars.Api.Device.Services;

using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.EventGrid;
using Azure.Security.KeyVault.Secrets;

public class MqttDevices : IMqttDevices
{
    public async Task<List<string>> GetDeviceNames(MqttNamespaceOptions mqttNamespaceOptions)
    {
        TokenCredential cred = new DefaultAzureCredential();
        var client = new ArmClient(cred);

        var kvUri = new Uri("https://kv-autonomouscars.vault.azure.net");
        var secretClient = new SecretClient(kvUri, new DefaultAzureCredential());
        var namespaceName = await secretClient.GetSecretAsync("MqttNamespaceName");
        var resourceGroupName = await secretClient.GetSecretAsync("MqttNamespaceResourceGroupName");
        var subscriptionId = await secretClient.GetSecretAsync("MqttNamespaceSubscriptionId");

        var eventGridNamespaceResourceId = EventGridNamespaceResource.CreateResourceIdentifier(subscriptionId.Value.Value, resourceGroupName.Value.Value, namespaceName.Value.Value);
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
