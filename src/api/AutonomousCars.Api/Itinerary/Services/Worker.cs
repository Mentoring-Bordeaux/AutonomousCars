using System.Xml;

namespace AutonomousCars.Api.Itinerary.Services;

using AutonomousCars.Api.Models;
using GeoJSON.Text.Geometry;
using GeoJSON.Text.Feature;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Extensions;
using System.Text.Json;

//https://github.com/dotnet/MQTTnet/blob/master/Samples/Client/Client_Publish_Samples.cs
public class Worker : BackgroundService, IWorker
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private bool _isStatusRequest;

    private String CAR_ID = "car01";
    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

    }
    public async Task ExecuteAsyncPublic(CancellationToken stoppingToken, bool isStatusRequest)
    {
        this._isStatusRequest = isStatusRequest;
        await ExecuteAsync(stoppingToken);
    }

    private Feature<LineString>? ComputeItineraryData()
    {
        string fileName = "Itinerary/Services/position.json";
        string jsonString = File.ReadAllText(fileName);
        TimePositionList? timePositionList= JsonSerializer.Deserialize<TimePositionList>(jsonString);
        var timePositions = timePositionList?.TimePositions;
        if (timePositions != null)
        {
            var positions = timePositions.Select(p => new Position(p.Longitude, p.Latitude)).ToArray();
            var positionsLineString = new LineString(positions);
            var properties = new Dictionary<string, object>();
            properties.Add("status", "false");
            return new Feature<LineString>(positionsLineString, properties);
        } 
        return null;
    }
    
    private Feature<LineString> ComputeStatusData()
    { ;
        var positions = new[] {new Position(0, 0), new Position(0, 0)};
        var positionsLineString = new LineString(positions);
        var properties = new Dictionary<string, object>();
        properties.Add("status", "true");
        return new Feature<LineString>(positionsLineString, properties);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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


        ItineraryTelemetryProducer telemetry = new(mqttClient, CAR_ID);


        var geospatialFeature = _isStatusRequest ? ComputeStatusData() : ComputeItineraryData();
        if (geospatialFeature != null)
        {
                
                MqttClientPublishResult pubAck = await telemetry.SendTelemetryAsync(geospatialFeature, stoppingToken);
                _logger.LogInformation("Message published with PUBACK {code} and mid {mid}", pubAck.ReasonCode, pubAck.PacketIdentifier);   
        }
        else
        {
            _logger.LogError("Failure of the Deserialization");
        }
    }
}