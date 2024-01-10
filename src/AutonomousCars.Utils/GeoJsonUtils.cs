namespace AutonomousCars.Utils;
public class GeoJsonUtils
{
    public static double GetDoubleProperty(IDictionary<string, object>? properties, string property)
    {
        if (properties != null && properties.TryGetValue(property, out var newProperty)  && double.TryParse(newProperty.ToString(), out var parsedProperty))
        { 
            return  parsedProperty;
        }
        return default;
    }

    public static string? GetStringProperty(IDictionary<string, object>? properties, string property)
    {
        if (properties != null && properties.TryGetValue(property, out var newProperty))
        { 
            return  newProperty.ToString();
        }
        return default;
    }
}