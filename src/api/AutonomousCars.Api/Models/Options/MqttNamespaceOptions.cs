namespace AutonomousCars.Api.Models.Options;

public class MqttNamespaceOptions
{
    public string SubscriptionId { get; }
    public string ResourceGroupName { get; }
    public string Namespace { get; }

    public MqttNamespaceOptions(string subscriptionId, string resourceGroupName, string namespaceName)
    {
        SubscriptionId = subscriptionId;
        ResourceGroupName = resourceGroupName;
        Namespace = namespaceName;
    }
}
