<script setup lang="ts">
import type { Vehicle } from '~/models/Vehicle';

const items = [
  { key: 'all', label: 'Tous' },
  { key: 'available', label: 'Disponible' },
  { key: 'unavailable', label: 'Indisponible' },
];

const vehiclesListStore = useVehiclesListStore();

function shouldDisplayVehicles(item: { key: string }) {
  if (item.key === 'all') {
    return vehiclesListStore.vehiclesList
  }else if (item.key === 'available'){
    return vehiclesListStore.vehiclesList.filter((vehicle) =>  vehicle.available == true);
  }
  else{
    return vehiclesListStore.vehiclesList.filter((vehicle) =>  vehicle.available == true);
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
      <div v-if="vehiclesListStore.isLoaded" class="space-y-3">
        <VehiclesCardList :vehiclesList="shouldDisplayVehicles(item)"></VehiclesCardList>
      </div>
      <div v-else>
        <VehiclesLoader></VehiclesLoader>
      </div>
    </template>
  </UTabs>
</template>