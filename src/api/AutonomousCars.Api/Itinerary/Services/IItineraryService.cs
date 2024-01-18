using System.Collections;
using AutonomousCars.Api.Models.Itinerary;

namespace AutonomousCars.Api.Itinerary.Services;

using GeoJSON.Text.Geometry;
using GeoJSON.Text.Feature;
public interface IItineraryService
{
    public bool CheckItineraryFormat(Feature<LineString> geospatialFeature);
    public Feature<LineString> ComputeItinerary(Feature<LineString> itinerary);
    public List<Feature<LineString>> ComputeStatusData(StatusRequestData statusRequestData);
    public Task SendRequest(CancellationToken stoppingToken, List<Feature<LineString>> geospatialFeatures, bool isStatusRequest);
  
}
