namespace AutonomousCars.Api.Controllers;

using Azure.Core;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TokenController : Controller
{
    private readonly TokenCredential _tokenCredential;

    public TokenController(TokenCredential tokenCredential)
    {
        _tokenCredential = tokenCredential;
    }

    [HttpGet("map")]
    public async Task<IActionResult> GetMapAccessToken()
    {
        var accessToken = await _tokenCredential.GetTokenAsync(new TokenRequestContext(new string[] { "https://atlas.microsoft.com/.default" }), CancellationToken.None);
        return Ok(accessToken);
    }
}
