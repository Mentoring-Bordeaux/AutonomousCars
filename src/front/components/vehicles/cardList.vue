<script setup lang="ts">
import type { Vehicle } from '~/models/Vehicle';
defineProps<{
  vehiclesList : Vehicle[]
  }>();
</script>

<template>
  <div v-for="vehicle in vehiclesList">
    <VehiclesCard :vehicle="vehicle"></VehiclesCard>
  </div>
</template>
