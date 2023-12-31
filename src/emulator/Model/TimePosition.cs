using System.Text.Json.Serialization;

namespace Emulator.Model;

public class TimePosition
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }
    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }
    public int Timestamp { get; set; }
}