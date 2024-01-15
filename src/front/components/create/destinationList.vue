<script setup lang="ts">
import { useRouter } from "vue-router"
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
const emit = defineEmits(['update:isInForm']);
const {formatDistance, formatDuration} = useDistanceTimeFormat();

const router = useRouter();
const isLoaded = ref<Boolean>(false);
const isError = ref<Boolean>(false); 
const fetchRoutes = ref<( (startPosition: Position, endPosition: Position) => Promise<routeFeature[] | null> | null)>();
const routesSelection = ref<routeFeature[]| null>();
const chosenRoute = ref<routeFeature>();
const chosenCar = ref<carItem>(props.chosenCar);
const isInForm = ref<Boolean>(false);



  onMounted(async () => {
      const api = await useAzureMapsRoutes();
      fetchRoutes.value = api.fetchRoutes;
      await fetchRoutesFromAPI();
  }); 

  async function fetchRoutesFromAPI(){
    isLoaded.value = false; 
    isError.value = false;
    try{
      if(fetchRoutes.value instanceof Function){
        routesSelection.value = await fetchRoutes.value(props.positions.start, props.positions.end);
        isLoaded.value = true;
      } 
    }catch (error){
      isError.value = true;
      isLoaded.value = true;
    }
  }

  // eslint-disable-next-line require-await
  async function sendRoute(){
    useFetch("api/itinerary/create", {
        method: "POST",
        headers: {
          'Content-Type': "application/json"
        }, 
        body: chosenRoute.value,
      });
  }


  function handleSubmit(){
    if(chosenRoute !== undefined && chosenRoute.value !== undefined && chosenCar.value.vehicle !== null){
        chosenRoute.value.properties.carId = chosenCar.value.vehicle.carId;
        sendRoute();
        router.push('/');
      }
   }

  function handleRouteSelection(route: routeFeature){
    chosenRoute.value === route ? chosenRoute.value = undefined: chosenRoute.value = route;
  }

  function handleLinkToForm(){
    isInForm.value = true;
    emit('update:isInForm', isInForm.value);
  }



</script>

<template>
  <div class="bg-gray-100 p-4 shadow">
    <div class="flex items-center">
        <i class="i-heroicons-chevron-left cursor-pointer" @click="handleLinkToForm"></i>
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
    <div v-if="isLoaded == false">
      <div class="fetch-error flex flex-col justify-center items-center mt-4">
        <i class="i-heroicons-arrow-path text-[#E36C39] text-5xl flex items-center animate-spin"></i>
      </div>
    </div>
    <div v-if="isError == true">
      <div class="fetch-error flex flex-col justify-center items-center mt-4">
          <i class="i-heroicons-arrow-path text-[#E36C39] text-5xl flex items-center mb-4 cursor-pointer" @click="fetchRoutesFromAPI"></i>
          <span class="text-sm text-bold">Erreur lors du chargement des données.</span>
          <span class="text-sm text-bold">Veuillez réessayer.</span>
      </div>
    </div>
    <div v-if="isLoaded == true && isError == false">
      <div class="suggested-routes">
        <div v-for="(route, index) in routesSelection" :key="index" class="route py-2 px-3 rounded-lg cursor-pointer hover:bg-gray-100">
          <div
            :class="chosenRoute === route ? 'bg-orange-100 text-white p-2 rounded shadow-inner hover:bg-orange-200 transition-shadow border border-orange-400' : 'bg-white shadow hover:shadow-md transition-shadow border border-gray-200'"
            class="route flex justify-between items-center rounded-lg  p-3 mb-2 last:mb-0"
            @click="handleRouteSelection(route)">
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
        <form @submit.prevent="handleSubmit">
          <div class="grid gap-2 grid-cols-2 mt-4">
            <button type="reset" class="p-2 bg-white text-[#E36C39] border border-[#E36C39] rounded-lg hover:bg-gray-200">Annuler</button>
            <button type="submit" class="p-2 bg-[#E36C39] text-white rounded-lg hover:bg-orange-600">Valider</button>
          </div>
        </form>
      </div>
    </div>

  </div>
</div>
</template>