<script setup lang="ts">
import * as atlas from "azure-maps-control";
import "azure-maps-control/dist/atlas.min.css";

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
});
</script>

<template>
  <div id="map" class="w-screen h-screen p-0 m-0"></div>
</template>
