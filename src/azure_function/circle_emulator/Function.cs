using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace csharp_isolated;

public class Functions
{

    // itinerary to make a circle in Bordeaux France
    private static readonly double[][] Coordinates = [[44.820900, -0.540887], [44.816910, -0.549084], [44.812411, -0.557402], [44.814767, -0.572660], [44.821979, -0.584090], [44.831374, -0.598355], [44.843274, -0.599613], [44.849691, -0.596497], [44.860680, -0.588156], [44.869431, -0.566570], [44.860315, -0.553862], [44.853657, -0.566392], [44.849896, -0.570178], [44.840642, -0.568950], [44.830790, -0.555252]];
    private static int IndexPosition = 0;

    [Function("negotiate")]
    public static HttpResponseData Negotiate([HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequestData req,
        [SignalRConnectionInfoInput(HubName = "serverless")] string connectionInfo)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json");
        response.WriteString(connectionInfo);
        return response;
    }

    [Function("broadcast")]
    [SignalROutput(HubName = "serverless")]
    public static SignalRMessageAction Broadcast([TimerTrigger("*/5 * * * * *")] TimerInfo timerInfo)
    {
        double[] toSend = Coordinates[IndexPosition];
        IndexPosition++;
        IndexPosition %= Coordinates.Length;
        return new SignalRMessageAction("newMessage", [toSend]);
    }
}