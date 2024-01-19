<script setup lang="ts">
import * as atlas from "azure-maps-control";
import * as signalR from "@microsoft/signalr";
import { useMapItineraries } from "~/composables/useMapItineraries"
import type { Vehicle } from "~/models/Vehicle";
import "azure-maps-control/dist/atlas.min.css";
import type { routeStoreItem } from "~/models/routeStoreItem";

const apiBaseUrl = "https://func-autonomouscars.azurewebsites.net";

const initialPosition = [-0.607294, 44.806267];

let currentPopup: atlas.Popup | null = null;

onMounted(async () => {
    const routesStore = useRoutesListStore();
    const { getMapCredential } = useAzureMaps();
    const { clientId, accessToken: { token } } = await getMapCredential();

    const map = new atlas.Map("map", {
        view: "Auto",
        language: "fr-FR",
        center: initialPosition,
        zoom: 10,
        authOptions: {
            authType: atlas.AuthenticationType.anonymous,
            clientId,
            getToken: (resolve) => resolve(token),
        },
    });
    

    // Wait until the map resources are ready.
    map.events.add('ready', function () {
            const connection = new signalR.HubConnectionBuilder()
            .withUrl(apiBaseUrl + '/api')
            .configureLogging(signalR.LogLevel.Information)
            .build();
            connection.on('newPosition', (message) => {
                
                const vehicles = useVehiclesListStore().vehiclesList;
                const vehicle = vehicles.find((vehicle) => vehicle.carId === message.carId);
        
                if(!vehicle) return;
                vehicle.available = true;

                let marker = vehicle.marker;
                marker.setOptions({position: message.position.coordinates.reverse()});

                // Attach a popup to the marker.
                const popup = new atlas.Popup({
                    content: '<div class="p-5 mx-4"><h2 class="pb-3 font-bold">Voiture : '+ vehicle.carId +'</h2>'
                        +
                        (vehicle.available ?
                            '<div class="p-2 mx-2 bg-orange-500 font-bold text-white rounded"><a href="/?tab=create">Planifier un trajet</a></div></div>' :
                            '<div class="pointer-events-none opacity-50 p-2 mx-2 bg-gray-400 font-bold text-black rounded"><a href="/?tab=create">Planifier un trajet</a></div></div>') +'</div>',
                    pixelOffset: [0, -40],
                    closeButton: true
                });

                map.events.add('click', marker, function (e) {
                    if (currentPopup) {
                        currentPopup.close();
                    }
                    popup.setOptions({
                        position: (e.target as atlas.HtmlMarker).getOptions().position
                    });
                    popup.open(map);
                    currentPopup = popup;
                });
                map.markers.add(marker);

            });
            connection.start()
            .catch(console.error);

    });

    const { addPointsOnMap, addRouteOnMap, removeSuggestedRoutes } = useMapItineraries();

    map.events.add('ready', function() {
        const dataSourceSuggestedRoute = new atlas.source.DataSource();
        const dataSourceUsedRoute = new atlas.source.DataSource();
        map.sources.add(dataSourceSuggestedRoute);
        map.sources.add(dataSourceUsedRoute);

        
        watch(() => routesStore.routes, (curr: routeStoreItem[], old: routeStoreItem[]) => {
            if(curr.length > old.length){
                const addedRoutes: routeStoreItem[] = curr.filter(x => !old.includes(x));
                const suggestedRoutes = addedRoutes.filter((route) => route.status === "suggested");                
                const usedRoutes = addedRoutes.filter((route) => route.status === "used");
                if(suggestedRoutes.length !== 0)        
                    addPointsOnMap(map, suggestedRoutes, dataSourceSuggestedRoute)
                    suggestedRoutes.forEach(route => { addRouteOnMap(map, route, dataSourceSuggestedRoute); });        
                if(usedRoutes.length !== 0)
                    addPointsOnMap(map, usedRoutes, dataSourceUsedRoute)
                    usedRoutes.forEach(route => { addRouteOnMap(map, route, dataSourceUsedRoute); });
                map.setCamera({
                    bounds: atlas.data.BoundingBox.fromLatLngs(addedRoutes[0].coordinates),
                    padding: 40
                });
                console.log('Route added on the map');
        }
        else if(curr.length < old.length){
            const removedRoutes: routeStoreItem[] = old.filter(x => !curr.includes(x));
            removeSuggestedRoutes(map, removedRoutes, dataSourceSuggestedRoute)
            console.log('Route deleted on the map');
        }
        else {
            console.log('Route modified');
        } 
            
    }, { deep: true });
    });
    
});

</script>

<template>
  <div id="map" class="h-[calc(100vh-56px)]"></div>
</template>
