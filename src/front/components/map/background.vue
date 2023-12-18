<script setup lang="ts">
import * as atlas from "azure-maps-control";
import "azure-maps-control/dist/atlas.min.css";
import * as signalR from "@microsoft/signalr"

const apiBaseUrl = "https://func-autonomouscars.azurewebsites.net";
const carPath = "/img/car.png"

const initialPosition = [-0.607294, 44.806267]

const carPosition = ref(initialPosition);
const carRotation = ref(0);

onMounted(() => {
	const config = useRuntimeConfig();
	const { getMapToken } = useAzureMaps();


	const map = new atlas.Map("map", {
		view: "Auto",
		language: "fr-FR",
		center: initialPosition,
		zoom: 10,
		authOptions: {
			authType: atlas.AuthenticationType.anonymous,
			clientId: config.public.azureMapsClientId,
			getToken: (resolve, reject) =>
			getMapToken()
			.then(({ token }) => resolve(token))
			.catch((error) => reject(error)),
		},
	});

	//Wait until the map resources are ready.
	map.events.add('ready', function () {
		var car = new atlas.HtmlMarker({
			position: initialPosition,
			htmlContent: '<img id="carImage" src="' + carPath + '" style="width: 50px; height: 30px; transform-origin: center center;" />'
		})
		map.markers.add(car)

		const connection = new signalR.HubConnectionBuilder()
		.withUrl(apiBaseUrl + '/api')
		.configureLogging(signalR.LogLevel.Information)
		.build();
		connection.on('newPosition', (message: atlas.data.Position) => {
			console.log("new value received: " + message);

			const oldPosition = carPosition.value;
			const newPosition = message.reverse();

			// Calculate the angle between the old and new positions.
			const angle = Math.atan2(newPosition[1] - oldPosition[1], newPosition[0] - oldPosition[0]) * (180 / Math.PI);

			// Update car rotation and position.
			carRotation.value = angle;
			carPosition.value = newPosition;

			// Apply rotation to the car image.
			const carImage = document.getElementById("carImage") as HTMLImageElement;
			carImage.style.transform = `rotate(${angle}deg)`;

			car.setOptions({
				position: newPosition
			})
		});
		connection.start()
		.catch(console.error);
	});
});
</script>

<template>
	<div id="map" class="h-[calc(100vh-4rem-12px)] w-screen"></div>
</template>
