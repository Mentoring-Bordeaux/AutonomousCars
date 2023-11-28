using AutonomousCars.Emulator.Model;
using System.ComponentModel.DataAnnotations;

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
            var lastTimePosition = timePositions.Last();
            foreach (var timePosition in timePositions)
            {
                MqttClientPublishResult pubAck = await telemetryPosition.SendTelemetryAsync(
                    new Point(new Position(timePosition.Latitude, timePosition.Longitude)), stoppingToken);
                
                _logger.LogInformation("Message published with PUBACK {code} and mid {mid}", pubAck.ReasonCode, pubAck.PacketIdentifier);

                if (timePosition != lastTimePosition)
                {
                    int nextPosition = timePositionList.timePositions.IndexOf(timePosition) + 1;
                    int timeToWait = timePositionList.timePositions[nextPosition].Timestamp - timePosition.Timestamp;
                    _logger.LogInformation("Time to wait {time}", timeToWait);
                    await Task.Delay(timeToWait, stoppingToken);
                }
            }
        }
        else
        {
            _logger.LogError("Failure of the Deserialization");
        }
    }
}