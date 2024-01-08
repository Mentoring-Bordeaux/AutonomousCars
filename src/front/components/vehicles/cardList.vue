<script setup lang="ts">
import type { Vehicle } from '~/models/Vehicle';

const enum Availability {
    All,
    Available,
    Unavailable
}
const props = defineProps({
    type: Number,
});

const type = ref(props.type)

const shouldDisplayVehicle = (vehicle: Vehicle): Boolean => {
  console.log("type " + type.value);
  console.log("ALL : " + Availability.All);
  console.log("AVAILABLE : " + Availability.Available);
  console.log("UNAVAILABLE : " + Availability.Unavailable);
  return type.value === Availability.All 
         || (type.value === Availability.Available && vehicle.available)
         || (type.value === Availability.Unavailable && !vehicle.available);
};

var vehiclesList = useVehiclesListStore().vehiclesList
</script>
<template>
    <div v-for="vehicle in vehiclesList">
        <VehiclesCard v-if="shouldDisplayVehicle(vehicle)" :vehicle=vehicle></VehiclesCard>
    </div>
</template>