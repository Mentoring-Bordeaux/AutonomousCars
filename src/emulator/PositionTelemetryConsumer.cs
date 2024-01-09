namespace Emulator;

using GeoJSON.Text.Geometry;
using GeoJSON.Text.Feature;
using MQTTnet.Client;
using MQTTnet.Client.Extensions;

public class PositionTelemetryConsumer : TelemetryConsumer<Feature<LineString>>
{
    public PositionTelemetryConsumer(IMqttClient mqttClient)
        : base(mqttClient, new Utf8JsonSerializer(), $"itinerary/{mqttClient.Options.ClientId}")
    {
    }
}