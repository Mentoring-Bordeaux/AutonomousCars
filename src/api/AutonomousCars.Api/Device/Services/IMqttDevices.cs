using AutonomousCars.Api.Models.Options;

namespace AutonomousCars.Api.Device.Services;

public interface IMqttDevices
{
    public Task<List<String>> GetDeviceNames(MqttNamespaceOptions mqttNamespaceOptions);
}
