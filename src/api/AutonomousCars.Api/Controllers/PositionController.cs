using Microsoft.AspNetCore.Http.HttpResults;

namespace AutonomousCars.Api.Controllers;

using Azure.Core;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PositionController: Controller
{
    [HttpPost("position")]
    public async Task<ActionResult> PostPosition(Models.Position pos)
    {
        Console.WriteLine("Longitude : " + pos.latitude + "Latitude : " + pos.longitude);
        return Ok("Value received");
    }
}