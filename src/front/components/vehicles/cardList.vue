<script setup lang="ts">

const props = defineProps(['vehiclesList']);
</script>

<template>
  <div v-for="vehicle in vehiclesList">
    <VehiclesCard :vehicle="vehicle"></VehiclesCard>
  </div>
</template>
