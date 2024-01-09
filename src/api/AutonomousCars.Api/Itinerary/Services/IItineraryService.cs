namespace AutonomousCars.Api.Itinerary.Services;

using GeoJSON.Text.Geometry;
using GeoJSON.Text.Feature;
public interface IItineraryService
{
    public bool CheckItineraryFormat(Feature<LineString> geospatialFeature);
    public Feature<LineString> ComputeItinerary(Feature<LineString> itinerary);
    public Feature<LineString> ComputeStatusData(string carId);
    public Task SendRequest(CancellationToken stoppingToken, Feature<LineString> geospatialFeature, bool isStatusRequest);
  
}
