namespace AutonomousCars.Api.Controllers;

using Models;
using Models.Exceptions;
using Models.Options;
using Azure.Core;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

[ApiController]
[Route("api/[controller]")]
public class CredentialsController(IOptions<AzureMapsOptions> azureMapsOptions, TokenCredential tokenCredential) : Controller
{
    private readonly AzureMapsOptions _azureMapsOptions = azureMapsOptions.Value;

    private static readonly string[] AtlasScopes = {"https://atlas.microsoft.com/.default"}; 

    [HttpGet("map")]
    public async Task<IActionResult> GetMapCredential()
    {
        var credential = new ResourceCredential
        {
            ClientId = _azureMapsOptions.ClientId ?? throw new MissingSettingException($"{nameof(AzureMapsOptions)}.{nameof(AzureMapsOptions.ClientId)}"),
            AccessToken = await tokenCredential.GetTokenAsync(new TokenRequestContext(scopes: AtlasScopes), CancellationToken.None)
        };
        return Ok(credential);
    }
}
