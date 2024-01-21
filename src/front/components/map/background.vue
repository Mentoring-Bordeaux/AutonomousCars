<script setup lang="ts">
import * as atlas from "azure-maps-control";
import * as signalR from "@microsoft/signalr";
import { useMapItineraries } from "~/composables/useMapItineraries"
import "azure-maps-control/dist/atlas.min.css";
import type { routeStoreItem } from "~/models/routeStoreItem";

const apiBaseUrl = "https://func-autonomouscars.azurewebsites.net";

const initialPosition = [-0.607294, 44.806267];

let currentPopup: atlas.Popup | null = null;

onMounted(async () => {
    const routesStore = useRoutesListStore();
    const { addRoutes, removeRoutes, changeRouteColor, updateRoute } = useMapItineraries();
    const { getMapCredential } = useAzureMaps();
    const { clientId, accessToken: { token } } = await getMapCredential();
    const toast = useToast();

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
            const dataSourceSuggestedRoute = new atlas.source.DataSource();
            const dataSourceUsedRoute = new atlas.source.DataSource();
            map.sources.add(dataSourceSuggestedRoute);
            map.sources.add(dataSourceUsedRoute);

            addRoutes(map, routesStore.routes, dataSourceSuggestedRoute, dataSourceUsedRoute);

            const connection = new signalR.HubConnectionBuilder()
            .withUrl(apiBaseUrl + '/api')
            .configureLogging(signalR.LogLevel.Information)
            .build();
            connection.on('newPosition', (message) => {
                
                const vehicles = useVehiclesListStore().vehiclesList;
                const vehicle = vehicles.find((vehicle) => vehicle.carId === message.carId);
                
                
                if(!vehicle) return;
                vehicle.available = true;

                const marker: atlas.HtmlMarker = vehicle.marker as atlas.HtmlMarker;
                marker.setOptions({position: message.position.coordinates});

                // Update color route during the journey
                const coveredPoints = routesStore.getRouteCoordinates(message.carId, message.position.coordinates);
                if(coveredPoints !== null)
                    updateRoute(map, coveredPoints, message.carId, dataSourceUsedRoute);

                // Watch the end of a journey in order to remove the layers
                const endPosition = routesStore.getEndPosition(message.carId)
                if(endPosition != null && message.position.coordinates[0] === endPosition[0] 
                    && message.position.coordinates[1] === endPosition[1]){
                        routesStore.removeRoute(message.carId);
                        toast.add({title:"Votre itinéraire est terminé"});

                }

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

        
        watch(() => routesStore.routes, (curr: routeStoreItem[], old: routeStoreItem[]) => {
            if(curr.length > old.length){
                const addedRoutes: routeStoreItem[] = curr.filter(x => !old.includes(x));
                addRoutes(map, addedRoutes, dataSourceSuggestedRoute, dataSourceUsedRoute);
                map.setCamera({
                    bounds: atlas.data.BoundingBox.fromLatLngs(addedRoutes[0].coordinates),
                    padding: 40
                });
            }
            else if(curr.length < old.length){
                const removedRoutes: routeStoreItem[] = old.filter(x => !curr.includes(x));
                removeRoutes(map, removedRoutes, dataSourceSuggestedRoute, dataSourceUsedRoute);
            }
            else {
                console.log('Route modified');
                const suggestedRoutes = curr.filter(route => route.status === "suggested");
                changeRouteColor(map, suggestedRoutes, dataSourceSuggestedRoute);   
         } 
            
        }, { deep: true });
    });
    
});

</script>

<template>
  <div id="map" class="h-[calc(100vh-56px)]"></div>
</template>
