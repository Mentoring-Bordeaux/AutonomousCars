namespace Emulator;

using Model;
using GeoJSON.Text.Geometry;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Extensions;
using System.Text.Json;

// https://github.com/dotnet/MQTTnet/blob/master/Samples/Client/Client_Publish_Samples.cs
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

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
        
        
        
        PositionTelemetryProducer telemetryProducer = new(mqttClient);
        PositionTelemetryConsumer telemetryConsumer = new(mqttClient)
        {
            OnTelemetryReceived = HandleRequest(telemetryProducer, stoppingToken)
        };
        await telemetryConsumer.StartAsync();
    }
    
    private const int Speed = 500; // Arbitrary number to send messages
    
    private Action<TelemetryMessage<LineString>> HandleRequest(PositionTelemetryProducer telemetryProducer, CancellationToken stoppingToken)
    {
        return Handle;
        
        async void Handle(TelemetryMessage<LineString> request)
        {
            _logger.LogInformation("Received request from {from}", request.ClientIdFromTopic);
         
            
            if (request.Payload!.Coordinates.Count > 0)
            {
                foreach (var point in request.Payload.Coordinates)
                {
                    await telemetryProducer.SendTelemetryAsync(new Point(new Position(point.Latitude, point.Longitude)), stoppingToken);
                    _logger.LogInformation("Time to wait {time}", Speed);
                    await Task.Delay(Speed, stoppingToken);
                }
            }
            else
            {
               SendDefaultPositions(telemetryProducer, stoppingToken);
            }
        }
    }

    private async void SendDefaultPositions(PositionTelemetryProducer telemetryProducer, CancellationToken stoppingToken)
    {
        const string fileName = "position.json";
        var jsonString = File.ReadAllText(fileName);
        var timePositionList= JsonSerializer.Deserialize<TimePositionList>(jsonString);
        if (timePositionList != null)
        {
            var timePositions = timePositionList.TimePositions;
            foreach (var timePosition in timePositions)
            {
                var pubAck = await telemetryProducer.SendTelemetryAsync(
                    new Point(new Position(timePosition.Latitude, timePosition.Longitude)), stoppingToken);

                _logger.LogInformation("Message published with PUBACK {code} and mid {mid}", pubAck.ReasonCode, pubAck.PacketIdentifier);

                _logger.LogInformation("Time to wait {time}", Speed);
                await Task.Delay(Speed, stoppingToken);
            }
        }
        else
        {
            _logger.LogError("Failure of the Deserialization");
        }
    }
}