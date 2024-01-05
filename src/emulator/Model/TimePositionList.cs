using System.Text.Json;
using System.Text.Json.Serialization;

namespace Emulator.Model;

public class TimePositionList
{
    [JsonPropertyName("timePositions")]
    public List<TimePosition> TimePositions { get; set; } = new();
}