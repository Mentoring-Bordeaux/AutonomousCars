import * as atlas from "azure-maps-control";
import type { routeStoreItem } from "~/models/routeStoreItem";

export function useMapItineraries() {

    function addPointsOnMap(map: atlas.Map, addedRoutes: routeStoreItem[], dataSource: atlas.source.DataSource){
        const startPoint = new atlas.data.Feature(new atlas.data.Point(addedRoutes[0].coordinates[0]), {
            icon: "pin-blue"
        });
        const endPoint = new atlas.data.Feature(new atlas.data.Point(addedRoutes[0].coordinates[addedRoutes[0].coordinates.length - 1]), {
            icon: "pin-round-blue"
        });
        dataSource.add([startPoint, endPoint]);
        map.layers.add(
            new atlas.layer.SymbolLayer(dataSource, `points_${addedRoutes[0].id}`, {
                iconOptions: {
                    image: ["get", "icon"],
                    allowOverlap: true,
                    ignorePlacement: true
                },
                textOptions: {
                    textField: ["get", "title"],
                    offset: [0, 1.2]
                },
                filter: ["any", ["==", ["geometry-type"], "Point"], ["==", ["geometry-type"], "MultiPoint"]] // Only render Point or MultiPoints in this layer.
            })
        );
    }

    function addRouteOnMap(map: atlas.Map, route: routeStoreItem, dataSource: atlas.source.DataSource){
        dataSource.add(new atlas.data.Feature(new atlas.data.LineString(route.coordinates), null, route.id));
        map.layers.add(new atlas.layer.LineLayer(
            dataSource, route.id, {
                strokeColor: '#035290', // '#2272B9',
                strokeWidth: 5,
                lineJoin: 'round',
                lineCap: 'round',
            })
        ); 
    }

    function removeSuggestedRoutes(map: atlas.Map, routes: routeStoreItem[], dataSource: atlas.source.DataSource){
        routes.forEach((route) => {
            map.layers.remove(route.id);
        });
        dataSource.clear();
    }

    function removeUsedRoutes(map: atlas.Map, routes: routeStoreItem[], dataSource: atlas.source.DataSource){
        routes.forEach((route) => {
            map.layers.remove(route.id);
            dataSource.remove(route.id);
            map.layers.remove(`points_${route.id}`);
            dataSource.remove(`points_${route.id}`);
        });
    }

    return { addPointsOnMap, addRouteOnMap, removeSuggestedRoutes, removeUsedRoutes }
}