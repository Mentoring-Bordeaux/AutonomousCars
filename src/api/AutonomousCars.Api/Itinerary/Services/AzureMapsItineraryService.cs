using AutonomousCars.Api.Models.Itinerary;

namespace AutonomousCars.Api.Itinerary.Services;

using AutonomousCars.Api.Models.Options;
using GeoJSON.Text.Geometry;
using GeoJSON.Text.Feature;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Extensions;
using Utils;
using System.Collections;


public class AzureMapsItineraryService : IItineraryService
{
    private readonly ILogger<AzureMapsItineraryService> _logger;
    private readonly MqttSettings _mqttSettings;

    private const string IdPropName = "carId";
    private const string TimePropName = "time";
    private const string DistancePropName = "distance";
    private const string SpeedPropName = "speed";
    private const string StatusPropName = "status";
    
    public AzureMapsItineraryService(ILogger<AzureMapsItineraryService> logger, MqttSettings mqttSettings)
    {
        _logger = logger;
        _mqttSettings = mqttSettings;
    }
    
    public bool CheckItineraryFormat(Feature<LineString> geospatialFeature)
    {
        IDictionary<string, object>? properties = geospatialFeature.Properties;
        var coordinates = geospatialFeature.Geometry.Coordinates;

        var carIdValid = GeoJsonUtils.GetStringProperty(properties, IdPropName) != null;
        var timeValid = GeoJsonUtils.GetDoubleProperty(properties, TimePropName) > 0.0;
        var distanceValid = GeoJsonUtils.GetDoubleProperty(properties, DistancePropName) > 0.0;
        var coordinatesValid = coordinates.Count > 0;
        
        return carIdValid && timeValid && distanceValid && coordinatesValid;
    }

    private static double ComputeSpeed(double time, double nbPoints)
    {
        return  time / nbPoints;
    }
    
    public Feature<LineString> ComputeItinerary(Feature<LineString> itinerary)
    {
        IDictionary<string, object>? properties = itinerary.Properties;
        var nbPoints = itinerary.Geometry.Coordinates.Count;
        properties.Add(StatusPropName, "false");
        var time = GeoJsonUtils.GetDoubleProperty(properties, TimePropName);
        properties.Add(SpeedPropName, ComputeSpeed(time, nbPoints));
        return itinerary;
    }   
    
    public List<Feature<LineString>> ComputeStatusData(StatusRequestData statusRequestData)
    {
        List<Feature<LineString>> statusRequests = new List<Feature<LineString>>();
        foreach (var carId in statusRequestData.CarIdentifiers ?? Array.Empty<string>())
        {
            var positions = new[] {new Position(0, 0), new Position(0, 0)};
            var positionsLineString = new LineString(positions);
            var properties = new Dictionary<string, object>
            {
                {StatusPropName, "true"},
                {IdPropName, carId}
            };
            statusRequests.Add(new Feature<LineString>(positionsLineString, properties));
        }
        return statusRequests;
    }

    public async Task SendRequest(CancellationToken stoppingToken,  List<Feature<LineString>> geospatialFeatures, bool isStatusRequest)
    {
        
        var cs = _mqttSettings.MqttConnectionSettings;
        if (cs == null){
            _logger.LogError("Error to open the connexion");
        }
        
        _logger.LogInformation("Connecting to {cs}", cs);

        var mqttClient = new MqttFactory().CreateMqttClient(MqttNetTraceLogger.CreateTraceLogger());
        mqttClient.DisconnectedAsync += e =>
        {
            _logger.LogInformation("Mqtt client disconnected with reason: {e}", e.Reason);
            return Task.CompletedTask;
        };

        var connAck = await mqttClient.ConnectAsync(new MqttClientOptionsBuilder().WithConnectionSettings(cs!).Build(), stoppingToken);
        _logger.LogInformation("Client {ClientId} connected: {ResultCode}", mqttClient.Options.ClientId, connAck.ResultCode);

        foreach (var geospatialFeature in geospatialFeatures)
        {
            var properties = geospatialFeature.Properties;
            var carId = GeoJsonUtils.GetStringProperty(properties, IdPropName);
            if (carId != null)
            {
                ItineraryTelemetryProducer telemetry = new(mqttClient, carId);
                MqttClientPublishResult pubAck = await telemetry.SendTelemetryAsync(geospatialFeature, stoppingToken);
                _logger.LogInformation("Message published with PUBACK {code} and mid {mid}", pubAck.ReasonCode, pubAck.PacketIdentifier);
            } else
            {
                _logger.LogError("Failure of the Deserialization");
            }
        }
        await mqttClient.DisconnectAsync();
    }
}   
