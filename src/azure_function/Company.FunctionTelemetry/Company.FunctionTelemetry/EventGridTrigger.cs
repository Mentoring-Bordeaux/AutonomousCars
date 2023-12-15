// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using Azure.Messaging;
using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
namespace Company.FunctionTelemetry;

public static class EventGridTrigger
{   
    [Function("EventGridTrigger")]
    [SignalROutput(HubName = "chat", ConnectionStringSetting = "SignalRConnection")]
    public static SignalRMessageAction Run([EventGridTrigger] CloudEvent position, FunctionContext context)
    {
        var logger = context.GetLogger("EventGridTrigger");
        logger.LogInformation("Position received : {message}", position.Data.ToString());
        return new SignalRMessageAction("newPosition")
        {
            // broadcast to all the connected clients without specifying any connection, user or group.
            Arguments = new []{position},
        };
    }
}

public class Position
{
    public string? Type { get; set; }

    public double[]? Coordinates { get; set; }
    
}
