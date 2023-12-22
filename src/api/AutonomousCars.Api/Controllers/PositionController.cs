using Microsoft.AspNetCore.Http.HttpResults;

namespace AutonomousCars.Api.Controllers;

using Azure.Core;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PositionController: Controller
{
    [HttpPost("position")]
    public IActionResult PostPosition([FromBody] Models.Position pos)
    {
        Console.WriteLine("Longitude : " + pos.lon + ' ' + "Latitude : " + pos.lat);
        return Ok("Value received");
    }
}