namespace AutonomousCars.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using AutonomousCars.Api.Itinerary.Services;
using System.Threading.Tasks;



[ApiController]
[Route("api/itinerary")]
public class ItineraryController : ControllerBase
{
    private readonly IWorker _worker;

    public ItineraryController(IWorker worker)
    {
        _worker = worker;
    }

    [HttpGet]
    public async Task<IActionResult> SendItinerary()
    {
        await _worker.ExecuteAsyncPublic(CancellationToken.None);
        return Ok("Itinerary sent successfully.");
    }
}

