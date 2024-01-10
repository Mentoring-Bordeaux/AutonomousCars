<script setup lang="ts">
import { useAzureMapsRoutes } from "~/composables/useAzureMapsRoutes"
import { useDistanceTimeFormat } from "~/composables/useDistanceTimeFormat"
import type { Position } from "~/models/address"
import type { routeFeature } from "~/models/routes" 
import type { carItem } from "~/models/dropdownCar"

const props = defineProps<{
  addresses: {start: string, end: string};
  positions: {start: Position, end: Position};
  chosenCar: carItem;
}>();

console.log("Valeur de la voiture dans la destinationList : ", props.chosenCar.name);
console.log("Valeur des addresses dans la liste : ", props.addresses.end);

const fetchRoutes = ref<( (startPosition: Position, endPosition: Position) => Promise<routeFeature[] | null> | null)>();
const routesSelection = ref<routeFeature[] | null>();
const chosenRoute = ref<routeFeature>()
const chosenCar = props.chosenCar;

const {formatDistance, formatDuration} = useDistanceTimeFormat();

  onMounted(async () => {
    try{
      const api = await useAzureMapsRoutes();
      fetchRoutes.value = api.fetchRoutes;
      if(fetchRoutes.value instanceof Function){
        routesSelection.value = await fetchRoutes.value(props.positions.start, props.positions.end);
    }
  }catch (error){
    console.error("Erreur lors de la récupération des routes:", error);
  }

  }); 

//   async function sendRoute() {
//   fetch('api/itinerary/create', {
//     method: 'POST',
//     headers: {
//         'Content-Type': 'application/json',
//     },
//     body: JSON.stringify(chosenRoute),
// })
// .then(response => response)
// .then(data => console.log(data))
// .catch((error) => console.error('Error:', error));
// }


   function handleSubmit(){
      if(chosenRoute !== undefined){
        console.log("Route send to the back");
      }
        // sendRoute();
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
  <div class="m-4">
  <div class="flex items-center px-4 py-2 bg-gray-100 border rounded border-lightgray">
    <CreateCarItem :car="chosenCar" />
   </div> 
  <div class="navigation-widget max-w-lg mx-auto bg-gray-100 text-black text-bold p-6 border rounded-lg mt-4 border-lightgray">
    <div class="current-location mb-5">
      <p class="text-gray-600">De {{ addresses.start }}</p>
      <p class="text-[#E36C39]">à {{ addresses.end }}</p>
    </div>
    <p class="text-semibold">Routes suggérées</p>
    <div class="suggested-routes">
      <div v-for="(route, index) in routesSelection" :key="index" class="route py-2 px-3 rounded-lg cursor-pointer hover:bg-gray-100">
        <div 
          class="route flex justify-between items-center rounded-lg bg-white border border-gray-200 p-3 mb-2 last:mb-0 shadow hover:shadow-md transition-shadow"
          @click="chosenRoute = route">
          <div class="flex-grow">
            <div class="route-name text-lg text-bold text-[#E36C39]">{{formatDuration(route.properties.time)}}</div>
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
  
    <form @submit.prevent="handleSubmit">
      <div class="grid gap-2 grid-cols-2 mt-4">
        <button type="reset" class="p-2 bg-white text-[#E36C39] border border-[#E36C39] rounded-lg hover:bg-gray-200">Annuler</button>
        <button type="submit" class="p-2 bg-[#E36C39] text-white rounded-lg hover:bg-orange-600">Valider</button>
      </div>
    </form>
  </div>
</div>
</template>