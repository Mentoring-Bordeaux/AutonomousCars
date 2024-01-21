import * as atlas from "azure-maps-control";
import type { routeStoreItem } from "~/models/routeStoreItem";

export function useMapItineraries() {

    function addPointsOnMap(map: atlas.Map, addedRoute: routeStoreItem, dataSource: atlas.source.DataSource){ 
        const startPoint = new atlas.data.Feature(new atlas.data.Point(addedRoute.coordinates[0]), {
            icon: "pin-blue"
        });
        const endPoint = new atlas.data.Feature(new atlas.data.Point(addedRoute.coordinates[addedRoute.coordinates.length - 1]), {
            icon: "pin-round-blue"
        });
        dataSource.add([startPoint, endPoint]);
        map.layers.add(
            new atlas.layer.SymbolLayer(dataSource, `points_${addedRoute.id}`, {
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

    function getUniqueRoutes(addedRoutes: routeStoreItem[]){
        const uniqueRoutes: routeStoreItem[] = [];
        const uniqueSetPoints = new Set();
        addedRoutes.forEach(route => {
            const key = route.coordinates[0].join(",");
            if(!uniqueSetPoints.has(key)){
                uniqueRoutes.push(route);
                uniqueSetPoints.add(key);
            }
        });
        return uniqueRoutes
    }

    function addMutiplesPointsOnMap(map: atlas.Map, addedRoutes: routeStoreItem[], dataSource: atlas.source.DataSource){
        const uniqueRoutes = getUniqueRoutes(addedRoutes);
        uniqueRoutes.forEach((route) => {
            addPointsOnMap(map, route, dataSource)
        });
    }

    function addRouteLayer(map: atlas.Map, routeId: string, dataSource: atlas.source.DataSource){
        map.layers.add(new atlas.layer.LineLayer(
            dataSource, routeId, {
                strokeColor: ['case', ['==', ['get', 'click'], true], '#0863BD', '#B1D6FA'],
                strokeWidth: 7,
                lineJoin: 'round',
                lineCap: 'round',
            })
        );
    }

    function addRouteOnMap(map: atlas.Map, route: routeStoreItem, dataSource: atlas.source.DataSource){
        dataSource.add(new atlas.data.Feature(new atlas.data.LineString(route.coordinates), {click: false}, route.id));
        addRouteLayer(map, route.id, dataSource);
    }

    function addRoutes(map: atlas.Map, addedRoutes: routeStoreItem[],  
        dataSourceSuggestedRoute: atlas.source.DataSource, dataSourceUsedRoute: atlas.source.DataSource){
        const suggestedRoutes = addedRoutes.filter((route) => route.status === "suggested");                
        const usedRoutes = addedRoutes.filter((route) => route.status === "used");
        if(suggestedRoutes.length !== 0)        
            addMutiplesPointsOnMap(map, suggestedRoutes, dataSourceSuggestedRoute)
            suggestedRoutes.forEach(route => { addRouteOnMap(map, route, dataSourceSuggestedRoute); });        
        if(usedRoutes.length !== 0)
            addMutiplesPointsOnMap(map, usedRoutes, dataSourceUsedRoute)
            usedRoutes.forEach(route => { addRouteOnMap(map, route, dataSourceUsedRoute); });
    }

    function removeSuggestedRoutes(map: atlas.Map, routes: routeStoreItem[], dataSource: atlas.source.DataSource){
        routes.forEach((route) => {
            map.layers.remove(route.id);
            if(route.id.includes('_1'))
                map.layers.remove(`points_${route.id}`);
        });
        dataSource.clear();
    }

    function removeUsedRoutes(map: atlas.Map, routes: routeStoreItem[], dataSource: atlas.source.DataSource){
        routes.forEach((route) => {
            map.layers.remove(route.id);
            dataSource.remove(route.id);
            map.layers.remove(`points_${route.id}`);
            dataSource.remove(`points_${route.id}`);
            map.layers.remove(`${route.id}_update`);
            dataSource.remove(`${route.id}_update`);
        });
    }

    function removeRoutes(map: atlas.Map, removedRoutes: routeStoreItem[],  
        dataSourceSuggestedRoute: atlas.source.DataSource, dataSourceUsedRoute: atlas.source.DataSource){
        const suggestedRoutes = removedRoutes.filter((route) => route.status === "suggested");
        const usedRoutes = removedRoutes.filter((route) => route.status === "used");
        if(suggestedRoutes.length !== 0)
            removeSuggestedRoutes(map, suggestedRoutes, dataSourceSuggestedRoute);
        if(usedRoutes.length !== 0)
            removeUsedRoutes(map, usedRoutes, dataSourceUsedRoute);
        
    }

    function changeRouteColor(map: atlas.Map, routes: routeStoreItem[], dataSource: atlas.source.DataSource){
        const routesTmp = [...routes]
        routesTmp.sort((a, b) => {
            if (a.click === b.click)
                return 0;
            return a.click ? 1 : -1;
        });
        routesTmp.forEach(((route) => {     
            dataSource.remove(route.id);
            dataSource.add(new atlas.data.Feature(new atlas.data.LineString(route.coordinates), {click: route.click}, route.id));
            if(route.click === true){
                map.layers.move(route.id);
            }
        }));   
    }

    function updateRoute(map: atlas.Map, coordinates: Array<[number, number]>, routeId: string, dataSource: atlas.source.DataSource){
        dataSource.remove(`${routeId}_update`);
        dataSource.add(new atlas.data.Feature(new atlas.data.LineString(coordinates), {click: true}, `${routeId}_update`));
        if(!map.layers.getLayerById(`${routeId}_update`))
            addRouteLayer(map, `${routeId}_update`, dataSource);
    }

    return { addRoutes, removeRoutes, changeRouteColor, updateRoute};
}