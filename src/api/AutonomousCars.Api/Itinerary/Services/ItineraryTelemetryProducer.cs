namespace AutonomousCars.Api.Itinerary.Services;

using GeoJSON.Text.Geometry;
using MQTTnet.Client;
using MQTTnet.Client.Extensions;

internal class ItineraryTelemetryProducer : TelemetryProducer<LineString>
{
    public ItineraryTelemetryProducer(IMqttClient mqttClient, String carId)
        : base(mqttClient, new Utf8JsonSerializer(), $"itinerary/{carId}")
    {
    }
}
