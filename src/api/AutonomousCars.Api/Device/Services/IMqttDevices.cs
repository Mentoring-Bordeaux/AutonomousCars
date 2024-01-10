namespace AutonomousCars.Api.Device.Services;

public interface IMqttDevices
{
    public Task<List<String>> GetDeviceNames(string subscriptionId, string resourceGroupName, string namespaceName);
}
