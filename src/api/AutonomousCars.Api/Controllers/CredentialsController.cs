namespace AutonomousCars.Api.Controllers;

using AutonomousCars.Api.Models;
using AutonomousCars.Api.Models.Exceptions;
using AutonomousCars.Api.Models.Options;

using Azure.Core;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

[ApiController]
[Route("api/[controller]")]
public class CredentialsController(IOptions<AzureMapsOptions> options, TokenCredential tokenCredential) : Controller
{
    private readonly AzureMapsOptions _azureMapsOptions = options.Value;

    private readonly TokenCredential _tokenCredential = tokenCredential;

    [HttpGet("map")]
    public async Task<IActionResult> GetMapCredential()
    {
        var credential = new ResourceCredential()
        {
            ClientId = _azureMapsOptions.ClientId ?? throw new MissingSettingException($"{nameof(AzureMapsOptions)}.{nameof(AzureMapsOptions.ClientId)}"),
            AccessToken = await _tokenCredential.GetTokenAsync(new TokenRequestContext(["https://atlas.microsoft.com/.default"]), CancellationToken.None),
        };

        return Ok(credential);
    }
}
