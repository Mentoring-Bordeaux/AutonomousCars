namespace AutonomousCars.Api.Itinerary.Services;

using AutonomousCars.Api.Models;
using GeoJSON.Text.Geometry;
using GeoJSON.Text.Feature;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Extensions;
using System.Text.Json;

public class AzureMapsItineraryService : IItineraryService
{
    private readonly ILogger<AzureMapsItineraryService> _logger;
    private readonly IConfiguration _configuration;
    
    private const string Id = "carId";
    private const string Time = "time";
    private const string Distance = "distance";
    private const string Speed = "speed";
    private const string Status = "status";
    
    public AzureMapsItineraryService(ILogger<AzureMapsItineraryService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }
    
    public bool CheckItineraryFormat(Feature<LineString> geospatialFeature)
    {
        IDictionary<string, object>? properties = geospatialFeature.Properties;
        var coordinates = geospatialFeature.Geometry.Coordinates;

        bool carIdValid = GetStringProperty(properties, Id) != null;
        bool timeValid = GetDoubleProperty(properties, Time) != 0.0;
        bool distanceValid = GetDoubleProperty(properties, Distance) != 0.0;
        bool coordinatesValid = coordinates.Count > 0;
        
        return carIdValid && timeValid && distanceValid && coordinatesValid;
    }

    private double ComputeSpeed(double time, double distance)
    {
        return distance / time;
    }
    
    public Feature<LineString> ComputeItinerary(Feature<LineString> itinerary)
    { ;
        IDictionary<string, object>? properties = itinerary.Properties;
        properties.Add(Status, "false");
        double time = GetDoubleProperty(properties, Time);
        double distance = GetDoubleProperty(properties, Distance);
        properties.Add(Speed, ComputeSpeed(time, distance));
        return itinerary;
    }   
    
    public Feature<LineString> ComputeStatusData(string carId)
    { ;
        var positions = new[] {new Position(0, 0), new Position(0, 0)};
        var positionsLineString = new LineString(positions);
        var properties = new Dictionary<string, object>();
        properties.Add(Status, "true");
        properties.Add(Id, carId);
        return new Feature<LineString>(positionsLineString, properties);
    }
    
    private static double GetDoubleProperty(IDictionary<string, object>? properties, string property)
    {
        if (properties != null && properties.TryGetValue(property, out var newProperty)  && double.TryParse(newProperty.ToString(), out var parsedProperty))
        { 
            return  parsedProperty;
        }
        return default;
    }

    private static string? GetStringProperty(IDictionary<string, object>? properties, string property)
    {
        if (properties != null && properties.TryGetValue(property, out var newProperty))
        { 
            return  newProperty.ToString();
        }
        return default;
    }

    
    public async Task SendRequest(CancellationToken stoppingToken,  Feature<LineString> geospatialFeature, bool isStatusRequest)
    {
        MqttConnectionSettings cs = MqttConnectionSettings.CreateFromEnvVars(_configuration.GetValue<string>("envFile"));
        _logger.LogInformation("Connecting to {cs}", cs);

        IMqttClient mqttClient = new MqttFactory().CreateMqttClient(MqttNetTraceLogger.CreateTraceLogger());
        mqttClient.DisconnectedAsync += e =>
        {
            _logger.LogInformation("Mqtt client disconnected with reason: {e}", e.Reason);
            return Task.CompletedTask;
        };

        MqttClientConnectResult connAck = await mqttClient.ConnectAsync(new MqttClientOptionsBuilder().WithConnectionSettings(cs).Build(), stoppingToken);
        _logger.LogInformation("Client {ClientId} connected: {ResultCode}", mqttClient.Options.ClientId, connAck.ResultCode);

        var properties = geospatialFeature.Properties;
        string? carId = GetStringProperty(properties, Id);
        if (carId != null)
        {
            ItineraryTelemetryProducer telemetry = new(mqttClient, carId);
            MqttClientPublishResult pubAck = await telemetry.SendTelemetryAsync(geospatialFeature, stoppingToken);
            _logger.LogInformation("Message published with PUBACK {code} and mid {mid}", pubAck.ReasonCode, pubAck.PacketIdentifier);
        } else
        {
            _logger.LogError("Failure of the Deserialization");
        }
        await mqttClient.DisconnectAsync();
    }
}   
