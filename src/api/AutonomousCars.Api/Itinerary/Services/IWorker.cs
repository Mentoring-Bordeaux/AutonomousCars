namespace AutonomousCars.Api.Itinerary.Services;

public interface IWorker
{
    public Task ExecuteAsyncPublic(CancellationToken stoppingToken, bool isStatusRequest);
}
