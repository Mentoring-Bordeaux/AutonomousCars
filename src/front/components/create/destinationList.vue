<script setup lang="ts">
import { useRouter } from "vue-router"
import { useAzureMapsRoutes } from "~/composables/useAzureMapsRoutes"
import { useDistanceTimeFormat } from "~/composables/useDistanceTimeFormat"
import { useRoutesListStore } from "~/stores/routesList"
import type { Position } from "~/models/address"
import type { Routes } from "~/models/routes" 
import type { carItem } from "~/models/dropdownCar"
import type { routeStoreItem } from "~/models/routeStoreItem";

const props = defineProps<{
	addresses: {start: string, end: string};
	positions: {start: Position, end: Position};
	chosenCar: carItem;
}>();
const emit = defineEmits(['update:isInForm']);
const {formatDistance, formatDuration} = useDistanceTimeFormat();

const router = useRouter();
const routesStore = useRoutesListStore();
const isLoaded = ref<Boolean>(false);
const isError = ref<Boolean>(false); 
const fetchRoutes = ref<( (startPosition: Position, endPosition: Position, chosenCar: carItem) => Promise<Routes[] | null> | null)>();
const routesSelection = ref<Routes[]| null>();
const chosenRoute = ref<Routes>();
const selectedRouteStoreItem = ref<routeStoreItem>();
const chosenCar = ref<carItem>(props.chosenCar as carItem);
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
				routesSelection.value = await fetchRoutes.value(props.positions.start, props.positions.end, chosenCar.value as carItem);
				isLoaded.value = true;
			} 
		}catch (error){
			isError.value = true;
			isLoaded.value = true;
		}
	}

	async function sendRoute(){
		const toast = useToast();
		await $fetch("api/itinerary/create", {
			method: "POST",
	        headers: {
	          'Content-Type': "application/json"
	        }, 
	        body: chosenRoute.value?.routeFeature,
			onResponse: ({ response }) => {
				if(response.status === 200){
					toast.add({title:"Votre itinéraire a été crée !"});
					if(selectedRouteStoreItem.value !== undefined)
						routesStore.addOneRoute(selectedRouteStoreItem.value);
						// setTimeout(() => {
						// 	if(selectedRouteStoreItem.value !== undefined)
						// 		routesStore.removeRoute(selectedRouteStoreItem.value.id);
						// 	toast.add({title:"Votre itinéraire est terminé !"});
						// }, chosenRoute.value ? chosenRoute.value.routeFeature.properties.time*1000 : 2000);
				} else  {
					toast.add({title:"Votre itinéraire n'a pas pu être crée !"});
				}
			}, 
		});
	}


	function handleSubmit(){
		if(chosenRoute !== undefined && chosenRoute.value !== undefined && chosenCar.value.vehicle !== undefined){
			selectedRouteStoreItem.value = routesStore.changeAndGetSelectedRoute(chosenRoute.value.id, chosenCar.value.vehicle.carId);
			routesStore.removeSuggestedRoutes();
			sendRoute();
			router.push('/');
		}
	 }

	function handleRouteSelection(route: Routes){
		chosenRoute.value === route ? chosenRoute.value = undefined: chosenRoute.value = route;
		routesStore.toggleClick(route.id);
	}

	function handleLinkToForm(){
		isInForm.value = true;
		routesStore.removeSuggestedRoutes();
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
				<div v-for="(route, index) in routesSelection" :key="route.id" class="route py-2 px-3 rounded-lg cursor-pointer hover:bg-gray-100">
					<div
						:class="chosenRoute === route ? 'bg-orange-100 text-white p-2 rounded shadow-inner hover:bg-orange-200 transition-shadow border border-orange-400' : 'bg-white shadow hover:shadow-md transition-shadow border border-gray-200'"
						class="route flex justify-between items-center rounded-lg  p-3 mb-2 last:mb-0"
						@click="handleRouteSelection(route)">
						<div class="flex-grow">
							<div class="route-name text-lg text-bold text-[#E36C39]">{{formatDuration(route.routeFeature.properties.time)}}</div>
							<div class="route-duration text-sm text-gray-600">
								{{ formatDistance(route.routeFeature.properties.distance) }}
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