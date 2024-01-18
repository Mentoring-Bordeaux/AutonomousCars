using AutonomousCars.Api.Models.Itinerary;
using GeoJSON.Text.Feature;
using GeoJSON.Text.Geometry;

namespace AutonomousCars.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Itinerary.Services;
using System.Threading.Tasks;



[ApiController]
[Route("api/itinerary")]
public class ItineraryController : ControllerBase
{
    private readonly IItineraryService _itineraryService;

    public ItineraryController(IItineraryService itineraryService)
    {
        _itineraryService = itineraryService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> SendItinerary(Feature<LineString> itinerary)
    {
        if (_itineraryService.CheckItineraryFormat(itinerary))
        {
            var itineraryToSend = _itineraryService.ComputeItinerary(itinerary);
            await _itineraryService.SendRequest(CancellationToken.None, new List<Feature<LineString>>{itineraryToSend}, false);
            return Ok("Itinerary sent successfully.");
        }
        else
        {
            return BadRequest(new { error = "Invalid itinerary format." });
        }
    }
    
    [HttpPost("status")]
    public async Task<IActionResult> SendStatusRequest(StatusRequestData statusRequestData)
    {
        List<Feature<LineString>> geospatialFeature = _itineraryService.ComputeStatusData(statusRequestData);
        await _itineraryService.SendRequest(CancellationToken.None, geospatialFeature,true);
        return Ok("Status request sent successfully.");
    }
}

