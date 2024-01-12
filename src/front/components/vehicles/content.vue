<script setup lang="ts">
import type { Vehicle } from '~/models/Vehicle';

const items = [
  { key: 'all', label: 'Tous' },
  { key: 'available', label: 'Disponible' },
  { key: 'unavailable', label: 'Indisponible' },
];

const vehiclesListStore = useVehiclesListStore();

function shouldDisplayVehicles(item: { key: string }) : Vehicle[] {
  if (item.key === 'all') {
    return vehiclesListStore.vehiclesList
  }else if (item.key === 'available'){
    return vehiclesListStore.vehiclesList.filter((vehicle) =>  vehicle.available == true);
  }
  else{
    return vehiclesListStore.vehiclesList.filter((vehicle) =>  vehicle.available == false);
  }
};
</script>

<template>
  <div class="bg-gray-100 p-4 shadow">
    <div class="flex items-center"> 
      <nuxt-link :to="{ path: '/'}" class="text-gray-600 hover:text-gray-800 focus:outline-none">
        <i class="i-heroicons-chevron-left"></i>
      </nuxt-link>
      <h1 class="text-xl text-[#E36C39] font-400 ml-4">Les véhicules</h1>
    </div>
  </div>

  <UTabs :items="items" class="w-full mt-3">
    <template #item="{ item }">
        <div v-if="!vehiclesListStore.isLoaded" class="h-[50vh] flex justify-center items-center">
          <VehiclesLoader></VehiclesLoader>
        </div>
        <div v-else-if="vehiclesListStore.isLoaded" class="space-y-3">
          <VehiclesCardList :vehiclesList="shouldDisplayVehicles(item)"></VehiclesCardList>
        </div>
        <div v-else="vehiclesListStore.isError || vehiclesListStore.isError" class="h-[50vh] flex justify-center items-center">
          <p>
            Le service est momentanément indisponible. Veuillez réessayer ultérieurement ;)
          </p>
        </div>
    </template>
  </UTabs>
</template>
