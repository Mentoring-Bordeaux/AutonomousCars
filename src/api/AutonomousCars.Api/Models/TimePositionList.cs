using System.Text.Json;
using System.Text.Json.Serialization;


namespace AutonomousCars.Api.Models;

public class TimePositionList
{
    [JsonPropertyName("timePositions")]
    public List<TimePosition> TimePositions { get; set; } = new();
}