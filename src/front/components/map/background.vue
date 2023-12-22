<script setup lang="ts">
import * as atlas from "azure-maps-control";
import "azure-maps-control/dist/atlas.min.css";

onMounted(async () => {
  const { getMapCredential } = useAzureMaps();
  const {
    clientId,
    accessToken: { token },
  } = await getMapCredential();

  const map = new atlas.Map("map", {
    view: "Auto",
    language: "fr-FR",
    center: [-0.604945545248392, 44.806516403595744],
    zoom: 10,
    authOptions: {
      authType: atlas.AuthenticationType.anonymous,
      clientId,
      getToken: (resolve) => resolve(token),
    },
  });
});
</script>

<template>
  <div id="map" class="h-[calc(100vh-4rem-12px)] w-screen"></div>
</template>
