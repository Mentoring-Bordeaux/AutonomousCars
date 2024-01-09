<script setup lang="ts">
import * as atlas from "azure-maps-control";
import * as signalR from "@microsoft/signalr";
import type { Vehicle } from "~/models/Vehicle";
import "/components/map/style.css";
import "azure-maps-control/dist/atlas.min.css";

const apiBaseUrl = "https://func-autonomouscars.azurewebsites.net";
const carPath = "/img/car_icon.png";

const initialPosition = [-0.607294, 44.806267];

const carPosition = ref(initialPosition);
const carRotation = ref(0);
const vehicles: Vehicle[] = [
    {
        carId: "Car1",
        position: {
            type: "Point",
            coordinates: [-0.5792, 44.8378]
        },
        available: true
    },
    {
        carId: "Car2",
        position: {
            type: "Point",
            coordinates: [-0.5664, 44.8374] 
        },
        available: false
    },
    {
        carId: "Car3",
        position: {
            type: "Point",
            coordinates: [-0.5811, 44.8305]
        },
        available: true
    }
];

let currentPopup: atlas.Popup | null = null;

onMounted(async () => {
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

    //Wait until the map resources are ready.
    map.events.add('ready', function () {
        vehicles.forEach(vehicle => {
            const carMarker = new atlas.HtmlMarker({
                position: vehicle.position.coordinates,
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

            map.events.add('click', carMarker, function () {
                if (currentPopup) {
                    currentPopup.close();
                }

                popup.setOptions({
                    position: carMarker.getOptions().position
                });
                popup.open(map);

                currentPopup = popup;
            });

            map.markers.add(carMarker);

            const connection = new signalR.HubConnectionBuilder()
            .withUrl(apiBaseUrl + '/api')
            .configureLogging(signalR.LogLevel.Information)
            .build();
            connection.on('newPosition', (message: Vehicle) => {
                console.log("new value received: " + message.position.coordinates);

                const oldPosition = vehicle.position.coordinates;
                const newPosition = message.position.coordinates;

                // Calculate the angle between the old and new positions.
                const angle = Math.atan2(newPosition[1] - oldPosition[1], newPosition[0] - oldPosition[0]) * (180 / Math.PI);

                // Update car rotation and position.
                carRotation.value = angle;
                vehicle.position.coordinates = newPosition;

                // Apply rotation to the car image.
                const carImage = document.getElementById("carImage") as HTMLImageElement;
                carImage.style.transform = `rotate(${angle}deg)`;

                carMarker.setOptions({
                    position: newPosition
                });
            });
            connection.start()
            .catch(console.error);
        });
    });
});
</script>

<template>
  <div id="map" class="h-[calc(100vh-56px)]"></div>
</template>
