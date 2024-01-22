namespace AutonomousCars.Api.Models.Options;

public class MqttConfiguration
{
    public string ClientId { get; set; }
    public bool CleanSession { get; set; }
    public string Hostname { get; set; }
    public int TcpPort { get; set; }
    public string Username { get; set; }
    public bool UseTLS { get; set; }
    public string ResourceGroupName { get; set; }
    public string SubscriptionId { get; set; }
    public string EventGridNamespace { get; set; }
}
