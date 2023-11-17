using AutonomousCars.Emulator.Model;

namespace AutonomousCars.Emulator;
using GeoJSON.Text.Geometry;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Extensions;
using System.Text.Json;

//https://github.com/dotnet/MQTTnet/blob/master/Samples/Client/Client_Publish_Samples.cs
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


        PositionTelemetryProducer telemetryPosition = new(mqttClient);

        string fileName = "position.json";
        string jsonString = File.ReadAllText(fileName);
        TimePositionList? timePositionList= JsonSerializer.Deserialize<TimePositionList>(jsonString);
        if (timePositionList != null)
        {
            var timePositions = timePositionList.timePositions;
            TimePosition timePosition;
            int timePositionCount = timePositions.Count;
            int i;
            for (i = 0; i < timePositionCount-1; ++i)
            {
                timePosition = timePositions[i];
                MqttClientPublishResult pubAck = await telemetryPosition.SendTelemetryAsync(
                    new Point(new Position(timePosition.Latitude, timePosition.Longitude)), stoppingToken);
                _logger.LogInformation("Message published with PUBACK {code} and mid {mid}", pubAck.ReasonCode, pubAck.PacketIdentifier);
                int nextTime = timePositions[i + 1].Timestamp - timePosition.Timestamp;
                _logger.LogInformation("Time to wait {time}", nextTime);
                await Task.Delay(nextTime, stoppingToken);
            }
            timePosition = timePositions[i];
            MqttClientPublishResult pubAckLast = await telemetryPosition.SendTelemetryAsync(
                new Point(new Position(timePosition.Latitude, timePosition.Longitude)), stoppingToken);
            _logger.LogInformation("Message published with PUBACK {code} and mid {mid}", pubAckLast.ReasonCode, pubAckLast.PacketIdentifier);
        }
        else
        {
            _logger.LogError("Failure of the Deserialization");
        }
    }
}