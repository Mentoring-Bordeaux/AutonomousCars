namespace AutonomousCars.Api.Itinerary.Services;

using GeoJSON.Text.Geometry;
using MQTTnet.Client;
using MQTTnet.Client.Extensions;

internal class PositionTelemetryProducer : TelemetryProducer<Point>
{
    public PositionTelemetryProducer(IMqttClient mqttClient)
        : base(mqttClient, new Utf8JsonSerializer(), $"itinerary/{mqttClient.Options.ClientId}")
    {
    }
}
