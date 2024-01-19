import MapsRoute, { type MapsRouteClient, isUnexpected, toColonDelimitedLatLonString} from "@azure-rest/maps-route";
import type { TokenCredential } from "@azure/core-auth";
import type { Position } from '~/models/address'
import { carItem } from "~/models/dropdownCar";
import type { Routes } from "~/models/routes";
import type { routeStoreItem } from "~/models/routeStoreItem";

export async function useAzureMapsRoutes(): Promise<{ fetchRoutes: (startPosition: Position, endPosition: Position, chosenCar: carItem) => Promise<Routes[]>}>{

    const { getMapCredential } = useAzureMaps();
    const routesStore = useRoutesListStore();
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

    async function fetchRoutes(startPosition: Position, endPosition: Position, chosenCar: carItem): Promise<Routes[]> {
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

        // TODO : Change id to have carID_index
        const routes: Routes[] = response.body.routes.map(({ legs }, index) => ({
            "id": `${chosenCar.vehicle !== undefined ? chosenCar.vehicle.carId : 'noId'}_${index + 1}`,
            "routeFeature": {
                "type": "Feature",
                "geometry": {
                    "type": "LineString",
                    "coordinates": legs[0].points.map(point => [point.longitude, point.latitude])
                },
                "properties": {
                    "distance": legs[0].summary.lengthInMeters, 
                    "time": legs[0].summary.travelTimeInSeconds,
                    "carId": `${chosenCar.vehicle !== undefined ? chosenCar.vehicle.carId : 'noId'}`,
                }
            }
        }));

        const tempRoutes = [] as  routeStoreItem[];
        routes.forEach(route => tempRoutes.push({
            "id": route.id, 
            "coordinates": route.routeFeature.geometry.coordinates, 
            "click": false,
            "status": "suggested",
        }));
        routesStore.addMultipleRoutes(tempRoutes);

        return routes;

    }

    return { fetchRoutes };

}