<script setup lang="ts">
import type { Vehicle } from '~/models/Vehicle';

defineProps<{
  vehicle : Vehicle
}>();
</script>

<template>
  <div class="card border rounded overflow-hidden shadow-md mx-4 my-8">
    <div class="flex">
      <div class="w-1/3 h-auto border-r  bg-gray-100 relative flex items-center justify-center">
        <div class="rounded-full overflow-hidden p-1">
          <img src="/img/car_icon.png" alt="Voiture" class="w-full h-full object-cover" />
        </div>
      </div>
      <div class="p-4">
        <h2 class="text-sm font-bold mb-2">{{ vehicle.carName }}</h2>
        <div :class="[{'bg-green-500': vehicle.available, 'bg-red-500': !vehicle.available}, 'badge text-white border-dotted border border-white rounded-full p-1 text-center text-xs w-[100px]']">   
          {{ vehicle.available ? 'Disponible' : 'Indisponible' }}
        </div>
      </div>
    </div>
  </div>
</template>
