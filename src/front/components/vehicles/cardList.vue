<script setup lang="ts">
import type { Vehicle } from '~/models/Vehicle';

const enum Availability {
    All,
    Available,
    Unavailable
}
const props = defineProps({'type':Number});

const shouldDisplayVehicle = (vehicle: Vehicle): boolean => {
  return props.type === Availability.All 
         || (props.type === Availability.Available && vehicle.available)
         || (props.type === Availability.Unavailable && !vehicle.available);
};

var vehiclesList = useVehiclesListStore().vehiclesList
</script>
<template>
    <div v-for="vehicle in vehiclesList">
        <VehiclesCard v-if="shouldDisplayVehicle(vehicle)" :vehicle=vehicle></VehiclesCard>
    </div>
</template>