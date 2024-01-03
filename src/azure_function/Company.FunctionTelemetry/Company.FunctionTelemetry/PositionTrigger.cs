// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using Microsoft.Azure.Functions.Worker;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
namespace Company.FunctionTelemetry;
using System.Text.Json;
using System.Text.Json.Serialization;

public class PositionTrigger
{   
    private readonly ILogger<PositionTrigger> _logger;

    public PositionTrigger(ILogger<PositionTrigger> logger)
    {
        _logger = logger;
    }
    
    private string Decode64(string dataToDecode)
    {
        byte[] bytes = Convert.FromBase64String(dataToDecode);
        string decodedString = System.Text.Encoding.UTF8.GetString(bytes);
        return decodedString;
    }
    
    private string GetCardId(string input)
    {
        string[] parts = input.Split('/');
        string result = parts.Length >= 2 ? parts[1] : input;
        return result;
    }
    
    [Function("PositionTrigger")]
    [SignalROutput(HubName = "chat", ConnectionStringSetting = "SignalRConnection")]
    public SignalRMessageAction? Run([ServiceBusTrigger("sbq-positions", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message, FunctionContext context)
    {
    	_logger.LogInformation("Message ID: {id}", message.MessageId);
  	 	_logger.LogInformation("Message Body: {body}", message.Body);
    	_logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);
        CloudEventBody? cloudEvent = JsonSerializer.Deserialize<CloudEventBody>(message.Body.ToString());
        if (cloudEvent.DataBase64 != null && cloudEvent.Subject != null)
        {
            String decodeString = Decode64(cloudEvent.DataBase64);
            String idCar = GetCardId(cloudEvent.Subject);       
            Position? position= JsonSerializer.Deserialize<Position>(decodeString);
            VehicleLocation vehicleLocation = new VehicleLocation(idCar, position);
            _logger.LogInformation("Data sent: {idCar} {latitude} {longitude}", vehicleLocation.carId, vehicleLocation.position.Coordinates[0], vehicleLocation.position.Coordinates[1]);
            return new SignalRMessageAction("newPosition")
            {
                // broadcast to all the connected clients without specifying any connection, user or group.
                Arguments = new[] { vehicleLocation },
            };
        } 
        return null;
    }

	[Function("negotiate")]
    public static HttpResponseData Negotiate([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequestData req,
        [SignalRConnectionInfoInput(HubName = "chat")] string connectionInfo)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");
        response.WriteString(connectionInfo);
        return response;
    }
}


public class CloudEventBody
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("data_base64")]
    public string? DataBase64 { get; set; }

    [JsonPropertyName("time")]
    public DateTime Time { get; set; }

    [JsonPropertyName("specversion")]
    public string? SpecVersion { get; set; }

    [JsonPropertyName("subject")]
    public string? Subject { get; set; }
}
public class Position
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    [JsonPropertyName("coordinates")]
    public double[]? Coordinates { get; set; }
    
}

public class VehicleLocation
{
    public String carId { get; set; }
    public Position? position { get; set; }
    
    public VehicleLocation(String carId, Position? position)
    {
        this.carId = carId;
        this.position = position;
    }
}
