import MapsRoute, { type MapsRouteClient, isUnexpected, toColonDelimitedLatLonString} from "@azure-rest/maps-route";
import type { TokenCredential } from "@azure/core-auth";
import type { Position } from '~/models/address'
import type { routeFeature } from "~/models/routes";

export async function useAzureMapsRoutes(): Promise<{ fetchRoutes: (startPosition: Position, endPosition: Position) => Promise<routeFeature[]>}>{

    const { getMapCredential } = useAzureMaps();

    
    const { clientId } = await getMapCredential();
  
    const tokenCredential: TokenCredential = {
        getToken: async () => {
        const { accessToken : {token, expiresOn} } = await getMapCredential();
        return {
          token,
          expiresOnTimestamp: expiresOn,
        };
      },
    }; 

    const client: MapsRouteClient = MapsRoute(tokenCredential, clientId);

    async function fetchRoutes(startPosition: Position, endPosition: Position): Promise<routeFeature[]> {
        const response = await client.path("/route/directions/{format}", "json").get({
            queryParameters: {
                query: toColonDelimitedLatLonString([
                    [startPosition.lat, startPosition.lon],
                    [endPosition.lat, endPosition.lon]
                ]),
                maxAlternatives: 2,
                travelMode: "car",
            }
        });

        if (isUnexpected(response)) {
            throw response.body.error;
        }

        const routes: routeFeature[] = []; 

        response.body.routes.forEach(({ legs }) => {
            const route: routeFeature = {
                "type": "Feature",
                "geometry": {
                    "type": "LineString",
                    "coordinates": legs[0].points.map(point => [point.longitude, point.latitude])
                },
                "properties": {
                    "distance": legs[0].summary.lengthInMeters, 
                    "time": legs[0].summary.travelTimeInSeconds,
                    "carId": "noId" // Vous pouvez ajuster cette valeur comme nécessaire
                }
            };
            routes.push(route)
        });

        return routes;

    }

    return { fetchRoutes };

}