namespace AutonomousCars.Api.Itinerary.Services;

using GeoJSON.Text.Geometry;
using MQTTnet.Client;
using MQTTnet.Client.Extensions;
using GeoJSON.Text.Feature;

internal class ItineraryTelemetryProducer : TelemetryProducer<Feature<LineString>>
{
    public ItineraryTelemetryProducer(IMqttClient mqttClient, String carId)
        : base(mqttClient, new Utf8JsonSerializer(), $"itinerary/{carId}")
    {
    }
}
