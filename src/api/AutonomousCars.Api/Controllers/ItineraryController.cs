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

    [HttpGet("create")]
    public async Task<IActionResult> SendItinerary()
    {
        await _worker.ExecuteAsyncPublic(CancellationToken.None, false);
        return Ok("Itinerary sent successfully.");
    }
    
    [HttpGet("status")]
    public async Task<IActionResult> SendStatusRequest()
    {
        await _worker.ExecuteAsyncPublic(CancellationToken.None, true);
        return Ok("Status request sent successfully.");
    }
}

