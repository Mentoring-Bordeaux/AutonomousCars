namespace Emulator;

using GeoJSON.Text.Geometry;
using MQTTnet.Client;
using MQTTnet.Client.Extensions;

public class PositionTelemetryConsumer : TelemetryConsumer<LineString>
{
    public PositionTelemetryConsumer(IMqttClient mqttClient)
        : base(mqttClient, new Utf8JsonSerializer(), $"itinerary/{mqttClient.Options.ClientId}")
    {
    }
}