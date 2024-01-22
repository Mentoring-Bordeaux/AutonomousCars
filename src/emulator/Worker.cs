namespace Emulator;

using System.Collections.ObjectModel;
using GeoJSON.Text.Feature;
using GeoJSON.Text.Geometry;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Extensions;
using AutonomousCars.Utils;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    private string _carId = "Undefined";
    
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
        _carId = mqttClient.Options.ClientId;
        
        PositionTelemetryProducer telemetryProducer = new(mqttClient);
        PositionTelemetryConsumer telemetryConsumer = new(mqttClient)
        {
            OnTelemetryReceived = HandleRequest(telemetryProducer, mqttClient.Options.ClientId, stoppingToken)
        };
        await telemetryConsumer.StartAsync();
    }

    private const int Speed = 500; // Arbitrary number to send messages
    private Position _lastLocationKnown = new(44.84416, -0.57859); // Arbitrary location to simulate geolocation. 
    private readonly SemaphoreSlim _handleSemaphore = new(1, 1); // Only one itinerary per car

    private Action<TelemetryMessage<Feature<LineString>>> HandleRequest(PositionTelemetryProducer telemetryProducer, string clientId, CancellationToken stoppingToken)
    {
        return Handle;
        
        async void Handle(TelemetryMessage<Feature<LineString>> request)
        {
            try
            {
                _logger.LogInformation("Received request from {from}", request.ClientIdFromTopic);
                
                var coordinates = request.Payload!.Geometry!.Coordinates;
                var properties = request.Payload!.Properties;

                if (request.ClientIdFromTopic != clientId)
                {
                    _logger.LogInformation("Request from {clientId} but I am {myId}", request.ClientIdFromTopic, clientId);
                    return;
                }

                if (IsStatusRequest(properties))
                {
                    await telemetryProducer.SendTelemetryAsync(new Point(_lastLocationKnown), stoppingToken);
                    return;
                }

                if (coordinates != null) SendReceivedPositions(telemetryProducer, coordinates, GetSpeed(properties), stoppingToken);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in Handle.");
            }
            
        }
    }

    private static double GetSpeed(IDictionary<string,object>? properties)
    {
        var carSpeed = GeoJsonUtils.GetDoubleProperty(properties, "speed");
        return carSpeed > 0 ? carSpeed : Speed;
    }
    
    private bool IsStatusRequest(IDictionary<string,object>? properties)
    {
        _logger.LogInformation("Status requested from {carId}", _carId);
        var statusPropertyValue = GeoJsonUtils.GetStringProperty(properties, "status") ?? "false";
        return statusPropertyValue.ToLower().Equals("true");
    }
    
    private async void SendReceivedPositions(PositionTelemetryProducer telemetryProducer, ReadOnlyCollection<IPosition> itineraryList, double carSpeed,
        CancellationToken stoppingToken)
    {
        if (await _handleSemaphore.WaitAsync(TimeSpan.Zero, stoppingToken))
        {
            try
            {
                foreach (var point in itineraryList)
                {
                    _logger.LogInformation("{carId} to [lat: {lat}, lon: {lon}] in {time}s", _carId, point.Latitude, point.Longitude, Math.Round(carSpeed/1000f, 2));
                    var position = new Position(point.Latitude, point.Longitude);

                    await telemetryProducer.SendTelemetryAsync(new Point(position), stoppingToken);
                    _lastLocationKnown = position;

                    await Task.Delay( (int) carSpeed*1000, stoppingToken);
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
}