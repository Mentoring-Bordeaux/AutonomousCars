<script setup lang="ts">
import type { Vehicle } from '~/models/Vehicle';

const items = [
  { key: 'all', label: 'Tous' },
  { key: 'available', label: 'Disponible' },
  { key: 'unavailable', label: 'Indisponible' },
];

const vehiclesListStore = useVehiclesListStore();

const shouldDisplayVehicle = (item: { key: string }) => (vehicle: Vehicle): boolean => {
  return (
    item.key === 'all' ||
    (item.key === 'available' && vehicle.available) ||
    (item.key === 'unavailable' && !vehicle.available)
  );
};

const filteredVehiclesList = (item: { key: any; }) => {
  if (item.key === 'all') {
    return vehiclesListStore.vehiclesList;
  }
  return vehiclesListStore.vehiclesList.filter(shouldDisplayVehicle(item));
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
        <VehiclesCardList :vehiclesList="filteredVehiclesList(item)"></VehiclesCardList>
      </div>
      <div v-else>
        <VehiclesLoader></VehiclesLoader>
      </div>
    </template>
  </UTabs>
</template>