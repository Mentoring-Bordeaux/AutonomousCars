namespace AutonomousCars.Api.Models.Options;

public class MqttNamespaceOptions
{
    public required string SubscriptionId { get; set; }
    public required string ResourceGroupName { get; set; }
    public required string NamespaceName { get; set; }
}
