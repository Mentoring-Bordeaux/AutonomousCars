namespace Emulator;

using GeoJSON.Text.Feature;
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
        var cs = MqttConnectionSettings.CreateFromEnvVars(_configuration.GetValue<string>("envFile"));
        _logger.LogInformation("Connecting to {cs}", cs);

        var mqttClient = new MqttFactory().CreateMqttClient(MqttNetTraceLogger.CreateTraceLogger());
        mqttClient.DisconnectedAsync += e =>
        {
            _logger.LogInformation("Mqtt client disconnected with reason: {e}", e.Reason);
            return Task.CompletedTask;
        };

        var connAck = await mqttClient.ConnectAsync(new MqttClientOptionsBuilder().WithConnectionSettings(cs).Build(), stoppingToken);
        _logger.LogInformation("Client {ClientId} connected: {ResultCode}", mqttClient.Options.ClientId, connAck.ResultCode);
        
        PositionTelemetryProducer telemetryProducer = new(mqttClient);
        PositionTelemetryConsumer telemetryConsumer = new(mqttClient)
        {
            OnTelemetryReceived = HandleRequest(telemetryProducer, mqttClient.Options.ClientId, stoppingToken)
        };
        await telemetryConsumer.StartAsync();
    }

    private const int Speed = 500; // Arbitrary number to send messages
    private Position _lastLocationKnown = new(44.84416, -0.57859); // Arbitrary location to simulate geolocation. 
    private readonly SemaphoreSlim _handleSemaphore = new(1, 1);

    private Action<TelemetryMessage<Feature<LineString>>> HandleRequest(PositionTelemetryProducer telemetryProducer, string clientId, CancellationToken stoppingToken)
    {
        return Handle;
        
        async void Handle(TelemetryMessage<Feature<LineString>> request)
        {
            try
            {
                if (await _handleSemaphore.WaitAsync(TimeSpan.Zero, stoppingToken))
                {
                    try
                    {
                        _logger.LogInformation("Received request from {from}", request.ClientIdFromTopic);
                        var data = request.Payload!.Geometry;
                        var properties = request.Payload.Properties;

                        if (request.ClientIdFromTopic != clientId)
                        {
                            _logger.LogInformation("Request from {clientId} but I am {myId}", request.ClientIdFromTopic, clientId);
                            return;
                        }

                        if (data != null && data.Coordinates.Count > 0)
                        {
                            foreach (var point in data.Coordinates)
                            {
                                var carSpeed = Speed;
                                if (properties.TryGetValue("speed", out var newCarSpeed) && int.TryParse(newCarSpeed.ToString(), out var parsedSpeed))
                                {
                                    carSpeed = parsedSpeed;
                                }
                    
                                var position = new Position(point.Latitude, point.Longitude);
                    
                                await telemetryProducer.SendTelemetryAsync(new Point(position), stoppingToken);
                                _lastLocationKnown = position;
                    
                                _logger.LogInformation("Time to wait {time}", carSpeed);
                                await Task.Delay(carSpeed, stoppingToken);
                            }
                        }
                        else
                        {
                            if (properties != null && properties.TryGetValue("status", out var isTestValue) && isTestValue.ToString()!.ToLower().Equals("true"))
                            {
                                await telemetryProducer.SendTelemetryAsync(new Point(_lastLocationKnown), stoppingToken);
                                _logger.LogInformation("Ask for {clientId} status", request.ClientIdFromTopic);
                            }
                            else if (data != null)
                            {
                                SendDefaultPositions(telemetryProducer, stoppingToken);
                            }
                            else
                            {
                                _logger.LogInformation("Not a good format");
                            }
                        }
                    }
                    finally
                    {
                        _handleSemaphore.Release();
                    }
                }
                else
                {
                    _logger.LogInformation("Unable to emulate: Another job is currently running");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Handle.");
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