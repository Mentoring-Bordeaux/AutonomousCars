using Azure.Core;

namespace AutonomousCars.Api.Models;

public class ResourceCredential
{
    public required string ClientId { get; set; }

    public required AccessToken AccessToken { get; set; }
}