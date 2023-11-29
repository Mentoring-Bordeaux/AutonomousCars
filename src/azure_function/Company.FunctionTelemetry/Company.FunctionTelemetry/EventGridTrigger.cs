// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.FunctionTelemetry;

public static class EventGridTrigger
{   
    [Function("EventGridTrigger")]
    public static void Run([EventGridTrigger] Position position, FunctionContext context)
    {
        var logger = context.GetLogger("EventGridTrigger");
        logger.LogInformation("New point : {x}, {y}", position.Coordinates[0], position.Coordinates[1]);
    }
}

public class Position
{
    public string? Type { get; set; }

    public double[]? Coordinates { get; set; }
    
}
