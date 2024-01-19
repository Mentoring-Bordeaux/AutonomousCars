namespace AutonomousCars.Api.Controllers;

using Models;
using Models.Exceptions;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;

[ApiController]
[Route("api/[controller]")]
public class CredentialsController(TokenCredential tokenCredential) : Controller
{
    private static readonly string[] AtlasScopes = {"https://atlas.microsoft.com/.default"}; 

    [HttpGet("map")]
    public async Task<IActionResult> GetMapCredential()
    {
        
        var kvUri = new Uri("https://kv-autonomouscars.vault.azure.net");
        var secretClient = new SecretClient(kvUri, new DefaultAzureCredential());
        var azureMapsClientId = await secretClient.GetSecretAsync("AzureMapsClientID");

        var credential = new ResourceCredential
        {
            ClientId = azureMapsClientId.Value.Value ?? throw new MissingSettingException($"AzureMapsClientId.{nameof(azureMapsClientId)}"),
            AccessToken = await tokenCredential.GetTokenAsync(new TokenRequestContext(scopes: AtlasScopes), CancellationToken.None)
        };
        return Ok(credential);
    }
}
