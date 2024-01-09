<script setup lang="ts">
import type { RouteLegOutput} from "@azure-rest/maps-route";
import { useAzureMapsRoutes } from "~/composables/useAzureMapsRoutes"
import type { Position } from "~/models/address"
import type { routeFeature } from "~/models/routes" 

const props = defineProps<{
  startPosition: Position, 
  endPosition: Position
}>();

const startPostion = 0;
const endPosition = 0;

const fetchRoutes = ref<( (startPosition: Position, endPosition: Position) => Promise<routeFeature[] | null> | null)>();
const routesSelection = ref<routeFeature[] | null>();
const chosenRoute = ref<routeFeature>()

  onMounted(async () => {
    try{
    const api = await useAzureMapsRoutes();
    fetchRoutes.value = api.fetchRoutes;
    if(fetchRoutes.value instanceof Function){
      routesSelection.value = await fetchRoutes.value(props.startPosition, props.endPosition);
      console.log(routesSelection.value);
    }
  }catch (error){
    console.error("Erreur lors de la récupération des routes:", error);
  }

  }); 


  watch(chosenRoute, (newValue) => {
    console.log("La route a été enregistrée : ", newValue);
  })

  // function handleSubmit(){
  //   if(chosenRoute !== undefined)

  // }

  function formatDuration(seconds: number): string {
    if(seconds < 60){
      return `${seconds}s`;
    }else if(seconds < 3600){
      const minutes = Math.floor(seconds / 60);
      return `${minutes} min}`;
    }else if (seconds < 86400) {
        const hours = Math.floor(seconds / 3600);
        const remainingMinutes = Math.floor((seconds % 3600) / 60);
        return `${hours} h ${remainingMinutes > 0 ? `${remainingMinutes} min` : ''}`;
    } else {
        const days = Math.floor(seconds / 86400);
        const remainingHours = Math.floor((seconds % 86400) / 3600);
        return `${days} jours ${remainingHours > 0 ? ` et ${remainingHours} h` : ''}`;
    }
  }

  function formatDistance(meters: number): string {
    if(meters < 1000)
      return `${meters} mètres`;
    else{
      const kilometers = Math.floor(meters/1000)
      return `${kilometers} kilomètres`;
    }
  }


</script>

<template>
  <div class="bg-gray-100 p-4 shadow">
    <div class="flex items-center">
      <nuxt-link :to="{ path: '/'}" class="text-gray-600 hover:text-gray-800 focus:outline-none">
        <i class="i-heroicons-chevron-left"></i>
      </nuxt-link>
      <h1 class="text-xl text-[#E36C39] font-400 ml-4">Choisir un trajet</h1>
    </div>
  </div>
  <div class="p-4">
  <div class="navigation-widget max-w-lg mx-auto bg-gray-100 text-black text-bold p-6 border rounded-lg border-gray-300">
    <div class="current-location mb-5">
      <p class="text-gray-600">De votre position actuel</p>
      <p class="text-orange-500">à 18 Cours Victor Hugo 33000, Bordeaux</p>
    </div>
    <p class="text-semibold">Routes suggérées</p>
    <div class="suggested-routes">
      <div v-for="(route, index) in routesSelection" :key="index" class="route py-2 px-3 rounded-lg cursor-pointer hover:bg-gray-100">
        <div 
          class="route flex justify-between items-center rounded-lg bg-white border border-gray-200 p-3 mb-2 last:mb-0 shadow hover:shadow-md transition-shadow"
          @click="chosenRoute = route">
          <div class="flex-grow">
            <div class="route-name text-lg text-bold text-[#E36C39]">{{formatDuration(route.properties.temps)}}</div>
            <div class="route-duration text-sm text-gray-600">
              {{ formatDistance(route.properties.distance) }}
          </div>
          </div>
          <div v-if="index == 0" class="route-fastest text-sm text-gray-600">
              Le plus rapide
          </div>
        </div>
      </div>
    </div>
  
    <div class="grid gap-2 grid-cols-2 mt-4">
          <button type="reset" class="p-2 bg-white text-orange-500 border border-orange-500 rounded-lg hover:bg-gray-200">Annuler</button>
          <button type="submit" class="p-2 bg-orange-500 text-white rounded-lg hover:bg-orange-600">Valider</button>
        </div>
  </div>
</div>
</template>