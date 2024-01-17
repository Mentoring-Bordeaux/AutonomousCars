<script setup lang="ts">
import * as atlas from "azure-maps-control";
import * as signalR from "@microsoft/signalr";
import type { Vehicle } from "~/models/Vehicle";
import "~/components/map/style.css";
import "azure-maps-control/dist/atlas.min.css";

const apiBaseUrl = "https://func-autonomouscars.azurewebsites.net";

const initialPosition = [-0.607294, 44.806267];
const carPath = "/img/car_icon.png";

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
            connection.on('newPosition', (message: Vehicle) => {

                console.log("["+ message.carId +"] new value received: " + message.position.coordinates);
                
                const vehicles: Vehicle[] = useVehiclesListStore().vehiclesList;
                const vehicle = vehicles.find((vehicle) => vehicle.carId === message.carId);
         
                if(!vehicle) return;
                vehicle.available = true;

                let carMarker = map.markers.getMarkers().find((marker) => marker.getOptions().text === message.carId)
                if(carMarker){
                    carMarker.setOptions({position: message.position.coordinates});
                    return;
                }
                carMarker = new atlas.HtmlMarker({
                    text: message.carId,
                    position: message.position.coordinates,
                    htmlContent: '<div class="roundMarker">' +
                        '<img id="carImage" src="' + carPath + '" class="vehicleIcon"/>' +
                        (vehicle.available ?
                            '<div class="availability" style="background-color: green;"></div>' :
                            '<div class="availability" style="background-color: red;"></div>') +'</div>'
                });

                // Attach a popup to the marker.
                const popup = new atlas.Popup({
                    content: '<div class="p-5 mx-4"><h2 class="pb-3 font-bold">Voiture : '+ vehicle.carId +'</h2>'
                        +
                        (vehicle.available ?
                            '<div class="p-2 mx-2 bg-orange-500 font-bold text-white rounded"><a href="/?tab=create">Planifier un trajet</a></div></div>' :
                            '<div class="disabled p-2 mx-2 bg-gray-400 font-bold text-black rounded"><a href="/?tab=create">Planifier un trajet</a></div></div>') +'</div>',
                    pixelOffset: [0, -40],
                    closeButton: true
                });

                map.events.add('click', carMarker, function (e) {
                    if (currentPopup) {
                        currentPopup.close();
                    }
                    popup.setOptions({
                        position: (e.target as atlas.HtmlMarker).getOptions().position
                    });
                    popup.open(map);
                    currentPopup = popup;
                });

                map.markers.add(carMarker);
            });
            connection.start()
            .catch(console.error);

    });

    map.events.add('ready', function() {
        routesStore.removeAllRoute();
        const dataSource = new atlas.source.DataSource();
        map.sources.add(dataSource);

        watch(() => routesStore.routes, (curr, old) => {
        if(curr.length > old.length){
            const addedRoutes = curr.filter(x => !old.includes(x));
            console.log('Route added in the store');
            const startPoint = new atlas.data.Feature(new atlas.data.Point(addedRoutes[0].coordinates[0]), {
                icon: "pin-blue"
            });
            const endPoint = new atlas.data.Feature(new atlas.data.Point(addedRoutes[0].coordinates[addedRoutes[0].coordinates.length - 1]), {
                icon: "pin-round-blue"
            });
            dataSource.add([startPoint, endPoint]);
            map.layers.add(
                new atlas.layer.SymbolLayer(dataSource, undefined, {
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
            // const myRoutes = [] as atlas.data.Feature<atlas.data.LineString, Array<[number, number]>>[];
            addedRoutes.forEach(element => {
                dataSource.add(new atlas.data.Feature(new atlas.data.LineString(element.coordinates), null, `${element.id}-line`));
            });
            // dataSource.add(myRoutes);
            const colours = ['#035290', '#6B0303', '#A50ED9']
            addedRoutes.forEach(element => {
                map.layers.add(new atlas.layer.LineLayer(
                    dataSource, `${element.id}-line`, {
                        strokeColor: colours[parseInt(element.id) - 1], // '#2272B9',
                        strokeWidth: 5,
                        lineJoin: 'round',
                        lineCap: 'round',
                    })
                );    
            });

            map.setCamera({
                bounds: atlas.data.BoundingBox.fromLatLngs(addedRoutes[0].coordinates),
                padding: 40
            });
        }
        else if(curr.length < old.length){
            const removedRoutes = curr.filter(x => !curr.includes(x));
            removedRoutes.forEach(route => {
                map.layers.remove(`${route.id}-line`);
            });
            dataSource.clear();
            console.log('Route deleted in the store');
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
