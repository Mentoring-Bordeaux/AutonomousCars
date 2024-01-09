<script setup lang="ts">
import * as atlas from "azure-maps-control";
import * as signalR from "@microsoft/signalr";

import type { Vehicle } from "~/models/Vehicle";

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
                htmlContent: '<div style="position: relative; width: 45px; height: 45px; border-radius: 50%; overflow: hidden; background-color: #ececec; box-shadow: 0 0 5px rgba(0, 0, 0, 0.5); display: flex; align-items: center; justify-content: center; border: 1px solid white;">' +
					'<img id="carImage" src="' + carPath + '" style="width: 45px; height: 45px; object-fit: contain; border-radius: 50%;"/>' +
					(vehicle.available ?
						'<div style="position: fixed; bottom: 1px; right: 2px; width: 12px; height: 12px; border-radius: 50%; background-color: green; border: 2px solid white;"></div>' :
						'<div style="position: fixed; bottom: 1px; right: 2px; width: 12px; height: 12px; border-radius: 50%; background-color: red; border: 2px solid white;"></div>'
					) +'</div>'

            });
            map.markers.add(carMarker);

            const connection = new signalR.HubConnectionBuilder()
            .withUrl(apiBaseUrl + '/api')
            .configureLogging(signalR.LogLevel.Information)
            .build();
            connection.on('newPosition', (message: Vehicle) => {
                console.log("new value received: " + message.position.coordinates);

                const oldPosition = carPosition.value;
                const newPosition = message.position.coordinates;

                // Calculate the angle between the old and new positions.
                const angle = Math.atan2(newPosition[1] - oldPosition[1], newPosition[0] - oldPosition[0]) * (180 / Math.PI);

                // Update car rotation and position.
                carRotation.value = angle;
                carPosition.value = newPosition;

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
