<script setup lang="ts">
import * as atlas from "azure-maps-control";
import "azure-maps-control/dist/atlas.min.css";
import * as signalR from "@microsoft/signalr"

const apiBaseUrl = "http://localhost:7071";
const carPath = "/img/car.png"


onMounted(() => {
	const config = useRuntimeConfig();
	const { getMapToken } = useAzureMaps();


	const map = new atlas.Map("map", {
		view: "Auto",
		language: "fr-FR",
		center: [-0.604945545248392, 44.806516403595744],
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
			position: [-0.607294, 44.806267],
			htmlContent: '<img src="' + carPath + '" style="width: 50px; height: 30px;" />'
		})
		map.markers.add(car)

		const connection = new signalR.HubConnectionBuilder()
		.withUrl(apiBaseUrl + '/api')
		.configureLogging(signalR.LogLevel.Information)
		.build();
		connection.on('newMessage', (message: atlas.data.Position) => {
			car.setOptions({
				position: message.reverse()
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
